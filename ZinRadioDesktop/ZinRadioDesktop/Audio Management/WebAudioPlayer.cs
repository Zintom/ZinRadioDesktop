using NAudio;
using NAudio.Wave;
using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ZinRadioDesktop
{
    public sealed class WebAudioPlayer
    {

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

        private WaveOut? _waveOut;
        private BufferedWaveProvider? _bufferedWaveProvider = null;

        private readonly AsyncLock _mediaControlLocker = new AsyncLock();
        private readonly AsyncManualResetEvent _playExited = new AsyncManualResetEvent(false);
        private bool _playerPleaseExit = false;

        public async Task Stop()
        {
            using (await _mediaControlLocker.LockAsync())
            {
                if (AudioPlaybackState == PlaybackState.Stopped)
                {
                    return;
                }

                _playerPleaseExit = true;
                await _playExited.WaitAsync();
            }
        }

        public void Play(string url)
        {
            if (!Monitor.TryEnter(_mediaControlLocker))
            {
                return;
            }
            else if (AudioPlaybackState == PlaybackState.Playing)
            {
                Debug.WriteLine("An attempt to play was made before calling Stop().");
                return;
            }

            AudioPlaybackState = PlaybackState.Playing;
            _playerPleaseExit = false;
            _playExited.Reset();

            Monitor.Exit(_mediaControlLocker);

            _waveOut = new WaveOut();
            new Thread(new ThreadStart(async () =>
            {
                using (Stream stream = await _noCacheHttpClient.GetStreamAsync(url))
                using (ReadFullyStream readFullyAudioStream = new ReadFullyStream(stream))
                {
                    if (url.EndsWith(".mp3") || true)
                    {
                        IMp3FrameDecompressor? decompressor = null;

                        // Sizing calculated from info on https://id3.org/mp3Frame
                        byte[] decompressBuffer = new byte[16384 * 4];

                        do
                        {
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

                                _bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat)
                                {
                                    BufferDuration = TimeSpan.FromSeconds(20)
                                };

                                VolumeWaveProvider16 volumeProvider = new VolumeWaveProvider16(_bufferedWaveProvider);
                                volumeProvider.Volume = 1;

                                _waveOut.Init(volumeProvider);
                                _waveOut.Play();
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

                            if (_bufferedWaveProvider.BufferedDuration > _bufferedWaveProvider.BufferDuration - TimeSpan.FromSeconds(2))
                            {
                                break;
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
            })).Start();
        }

        public async Task Pause()
        {
            using (await _mediaControlLocker.LockAsync())
            {
                if (AudioPlaybackState == PlaybackState.Playing)
                {
                    _waveOut?.Pause();
                    AudioPlaybackState = PlaybackState.Paused;
                }
            }
        }

        public async Task Resume()
        {
            using (await _mediaControlLocker.LockAsync())
            {
                if (AudioPlaybackState == PlaybackState.Paused)
                {
                    _waveOut?.Play();
                    AudioPlaybackState = PlaybackState.Playing;
                }
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