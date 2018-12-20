using System;
using System.IO;
using AudioLibrary.Interfaces;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class AudioPlayer : IAudioPlayer
    {
        private WaveOut _player;
        private WAVEFORMATEX _format;
        private Stream _stream;
        private AudioDevice _device;

        public AudioPlayer(AudioDevice device)
        {
            _device = device;
        }

        public void Play(string fileName)
        {
            CloseFile();

            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.Open))
                using (var stream = new WaveStream(fileStream))
                {
                    if (stream.Length <= 0)
                        throw new Exception("Invalid WAV file");

                    _format = stream.Format;
                    if (_format.formatTag != WaveFormatTag.PCM && _format.formatTag != WaveFormatTag.IEEE_FLOAT)
                        throw new Exception("Olny PCM files are supported");

                    _stream = stream;
                }
            }
            catch
            {
                CloseFile();
            }

            if (_stream != null)
            {
                _stream.Position = 0;
                _player = new WaveOut(_device.DeviceId, _format, 16384, 3, new BufferFillEventHandler(Filler));
            }
        }

        public void Stop()
        {
            if (_player != null)
                try
                {
                    _player.Dispose();
                }
                finally
                {
                    _player = null;
                }
        }

        private void CloseFile()
        {
            Stop();

            if (_stream != null)
                try
                {
                    _stream.Close();
                }
                finally
                {
                    _stream = null;
                }
        }

        private void Filler(IntPtr data, int size)
        {
            byte[] b = new byte[size];
            if (_stream != null)
            {
                int pos = 0;
                while (pos < size)
                {
                    int toget = size - pos;
                    int got = _stream.Read(b, pos, toget);
                    if (got < toget)
                        _stream.Position = 0; // loop if the file ends
                    pos += got;
                }
            }
            else
            {
                for (int i = 0; i < b.Length; i++)
                    b[i] = 0;
            }
            System.Runtime.InteropServices.Marshal.Copy(b, 0, data, size);
        }

        public void Dispose()
        {
            CloseFile();
        }
    }

    internal delegate void BufferFillEventHandler(IntPtr data, int size);
}
