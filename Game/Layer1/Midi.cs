using System;
using System.Linq;
using System.Collections.Generic;
using Commons.Music.Midi;

namespace GameProject {
    public class Midi : IDisposable {
        public Midi() : this("") {}
        public Midi(string id) {
            var access = MidiAccessManager.Default;
            if (access.Outputs.Count() > 0) {
                foreach(IMidiPortDetails mpd in access.Outputs) {
                    Console.WriteLine(mpd.Name + " id: " + mpd.Id);
                }
                if (id.Count() > 0) {
                    _midiOut = access.OpenOutputAsync(access.Outputs.First(d => d.Id == id).Id).Result;
                } else {
                    _midiOut = access.OpenOutputAsync(access.Outputs.First().Id).Result;
                }
            }
        }

        public IMidiOutput Device => _midiOut;

        public static IEnumerable<IMidiPortDetails> Devices => MidiAccessManager.Default.Outputs;

        public void PlayNote(int noteNumber) {
            int channel = 0;

            NoteEvent note = _notesOn.FirstOrDefault(n => n.NoteNumber == noteNumber && n.Channel == channel);

            if (note == null) {
                note = new NoteEvent(channel, noteNumber);
                _notesOn.Add(note);
                _midiOut.Send(note.GetOnEvent(), 0, 3, 0);
            } else {
                _midiOut.Send(note.GetOffEvent(), 0, 3, 0);
                _midiOut.Send(note.GetOnEvent(), 0, 3, 0);
            }
        }
        public void StopNote(int noteNumber) {
            var note = _notesOn.FirstOrDefault(n => n.NoteNumber == noteNumber);
            if (note != null) {
                _midiOut.Send(note.GetOffEvent(), 0, 3, 0);
                _notesOn.Remove(note);
            }
        }
        public void StopAll() {
            foreach (NoteEvent n in _notesOn) {
                _midiOut.Send(n.GetOffEvent(), 0, 3, 0);
            }
            _notesOn.Clear();
        }

        public void Dispose() {
            StopAll();

            _midiOut.Dispose();
        }

        IMidiOutput _midiOut;

        HashSet<NoteEvent> _notesOn = new HashSet<NoteEvent>();

        private class NoteEvent {
            public NoteEvent(int channel, int noteNumber) {
                Channel = channel;
                NoteNumber = noteNumber;
            }

            // Max 16 channels (0 - 15)
            public int Channel {
                get;
                set;
            }
            public int NoteNumber {
                get;
                set;
            }
            public int Velocity {
                get;
                set;
            } = 80;

            public byte[] GetOnEvent() {
                return new byte[] { (byte)(0x90 + Channel), (byte)NoteNumber, (byte)Velocity };
            }
            public byte[] GetOffEvent() {
                return new byte[] { (byte)(0x80 + Channel), (byte)NoteNumber, (byte)Velocity };
            }
        }
    }
}
