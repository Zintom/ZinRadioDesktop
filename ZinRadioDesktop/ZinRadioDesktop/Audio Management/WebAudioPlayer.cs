using NAudio;
using NAudio.Dsp;
using NAudio.Wave;
using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ZinRadioDesktop.Plugins;

namespace ZinRadioDesktop
{
    public sealed class WebAudioPlayer
    {

        private bool userPaused = false;
        public enum PlaybackState
        {
            Playing,
            Paused,
            Stopped
        }

        public PlaybackState AudioPlaybackState { get; private set; } = PlaybackState.Stopped;

        private readonly HttpClient _noCacheHttpClient;

        private static readonly object _instanceLocker = new();
        private static WebAudioPlayer? _instance;
        public static WebAudioPlayer Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (_instanceLocker)
                {
                    if (_instance == null)
                    {
                        _instance = new WebAudioPlayer();
                    }
                }

                return _instance;
            }
        }

        private WebAudioPlayer()
        {
            var httpClientHandler = new HttpClientHandler();
            _noCacheHttpClient = new HttpClient(httpClientHandler);
            _noCacheHttpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
        }

        private WasapiOut? _waveOut;
        private BufferedWaveProviderEx? _bufferedWaveProvider = null;

        private readonly AsyncLock _mediaControlLocker = new AsyncLock();
        private readonly AsyncManualResetEvent _playExited = new AsyncManualResetEvent(false);
        private bool _playerPleaseExit = false;

        public async Task StopAsync()
        {
            if (AudioPlaybackState == PlaybackState.Stopped)
            {
                return;
            }

            userPaused = false;
            _playerPleaseExit = true;
            await _playExited.WaitAsync();
        }

        public async Task PlayAsync(string url)
        {
            using (await _mediaControlLocker.LockAsync())
            {
                if (AudioPlaybackState == PlaybackState.Playing)
                {
                    Debug.WriteLine("An attempt to play was made before calling Stop().");
                    return;
                }

                AudioPlaybackState = PlaybackState.Playing;
                userPaused = false;
                _playerPleaseExit = false;
                _playExited.Reset();
            }

            new Thread(new ThreadStart(() =>
            {
                _waveOut = new WasapiOut();

                using (Stream stream = _noCacheHttpClient.GetStreamAsync(url).ConfigureAwait(false).GetAwaiter().GetResult())
                using (ReadFullyStream readFullyAudioStream = new ReadFullyStream(stream))
                {
                    if (url.EndsWith(".mp3") || true)
                    {
                        IMp3FrameDecompressor? decompressor = null;

                        // Sizing calculated from info on https://id3.org/mp3Frame
                        byte[] decompressBuffer = new byte[16384 * 4];

                        do
                        {
                            if (IsBufferNearlyFull)
                            {
                                Debug.WriteLine("Streaming audio buffer is nearly full, sleeping for 500 milliseconds.");
                                Thread.Sleep(500);
                                continue;
                            }

                            Mp3Frame? frame = null;
                            try
                            {
                                frame = Mp3Frame.LoadFromStream(readFullyAudioStream);
                            }
                            catch (EndOfStreamException)
                            {
                                break;
                            }

                            if (frame == null) break;
                            if (decompressor == null)
                            {
                                // Initialize a Mp3 Decompressor based upon the frame.
                                decompressor = CreateMp3FrameDecompressor(frame);

                                _bufferedWaveProvider = new BufferedWaveProviderEx(decompressor.OutputFormat)
                                {
                                    BufferDuration = TimeSpan.FromSeconds(20),
                                    ReadFully = false
                                };

                                VolumeWaveProvider16 volumeProvider = new VolumeWaveProvider16(_bufferedWaveProvider);
                                volumeProvider.Volume = 1;

                                _waveOut.Init(volumeProvider);
                                //_waveOut.Play();
                                AudioPlaybackState = PlaybackState.Playing;
                            }

                            // The DecompressFrame method returns the number of bytes decompressed.
                            int bytesDecompressed = decompressor.DecompressFrame(frame, decompressBuffer, 0);

                            if (_bufferedWaveProvider == null)
                            {
                                break;
                            }

                            // Add the decompressed sample into the BWP.
                            _bufferedWaveProvider.AddSamples(decompressBuffer, 0, bytesDecompressed);

                            if (AudioPlaybackState == PlaybackState.Playing &&
                                _bufferedWaveProvider.BufferedDuration < TimeSpan.FromSeconds(1))
                            {
                                Pause(false);

                                Debug.WriteLine("Buffered data is less than 1 second. Pausing.");
                            }

                            if (!userPaused &&
                                AudioPlaybackState == PlaybackState.Paused &&
                                _bufferedWaveProvider.BufferedDuration >= TimeSpan.FromSeconds(1))
                            {
                                Resume();

                                Debug.WriteLine("Buffered data is now above 1 second. Resuming.");
                            }

                        } while (!_playerPleaseExit);

                        // Ensure the Mp3 Decompressor is disposed of.
                        decompressor?.Dispose();
                    }

                    _waveOut?.Dispose();
                    _bufferedWaveProvider = null;
                }

                AudioPlaybackState = PlaybackState.Stopped;

                // Release waiting threads.
                _playExited.Set();
            }))
            {
                Name = "Stream Download Thread"
            }.Start();

            //new Thread(new ThreadStart(() =>
            //{
            //    int fftLength = 2; // NAudio fft wants powers of two!
            //    SampleAggregator sampleAggregator = new SampleAggregator(fftLength);

            //    sampleAggregator.PerformFFT = true;
            //    sampleAggregator.FftCalculated += (args) =>
            //    {

            //        Debug.WriteLine($"FFT: {args.Result.Length}");
            //    };

            //    while (true)
            //    {
            //        if (_bufferedWaveProvider != null)
            //        {
            //            // Get the 16 bit sample
            //            byte[] next16BitSample = new byte[2];
            //            _bufferedWaveProvider.Peek(0, next16BitSample, 0, 2);

            //            // Convert to 32 bit for the FFT.
            //            float sample32 = AudioHelper.Convert16BitToFloat(next16BitSample)[0];

            //            // Add the sample to the aggregator to be converted.
            //            sampleAggregator.Add(sample32);
            //        }

            //        Thread.Sleep(16);
            //    }
            //})).Start();
        }

        /// <summary>
        /// <b>Do Not Use:</b> Use the relevant `<c>Task Pause()</c>` or `<c>void Pause()</c>`.
        /// </summary>
        private void PauseInternal(bool userPaused)
        {
            if (AudioPlaybackState == PlaybackState.Playing)
            {
                this.userPaused = userPaused;
                _waveOut?.Pause();
                AudioPlaybackState = PlaybackState.Paused;
            }
        }

        private void Pause(bool userPaused = false)
        {
            using (_mediaControlLocker.Lock())
            {
                PauseInternal(userPaused);
            }
        }

        public async Task PauseAsync()
        {
            using (await _mediaControlLocker.LockAsync())
            {
                PauseInternal(true);
            }
        }

        /// <summary>
        /// <b>Do Not Use:</b> Use the relevant `<c>Task Resume()</c>` or `<c>void Resume()</c>`.
        /// </summary>
        private void ResumeInternal()
        {
            if (AudioPlaybackState == PlaybackState.Paused)
            {
                userPaused = false;
                _waveOut?.Play();
                AudioPlaybackState = PlaybackState.Playing;
            }
        }

        private void Resume()
        {
            using (_mediaControlLocker.Lock())
            {
                ResumeInternal();
            }
        }

        public async Task ResumeAsync()
        {
            using (await _mediaControlLocker.LockAsync())
            {
                ResumeInternal();
            }
        }

        private bool IsBufferNearlyFull
        {
            get
            {
                return _bufferedWaveProvider != null &&
                       _bufferedWaveProvider.BufferLength - _bufferedWaveProvider.BufferedBytes
                       < _bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4;
            }
        }

        private static IMp3FrameDecompressor CreateMp3FrameDecompressor(Mp3Frame frame)
        {
            WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate,
                                                      frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                                                      frame.FrameLength,
                                                      frame.BitRate);

            return new AcmMp3FrameDecompressor(waveFormat);
        }

    }
}