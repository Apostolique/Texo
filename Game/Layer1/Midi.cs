using System;
using System.Linq;
using System.Collections.Generic;
using NAudio.Midi;

namespace GameProject {
    public class Midi : IDisposable {
        public Midi() {
            //List<string> devices = new List<string>();
            for (int i = 0; i < MidiOut.NumberOfDevices; i++) {
                //devices.Add(MidiOut.DeviceInfo(i).ProductName);
                Console.WriteLine(MidiOut.DeviceInfo(i).ProductName);
            }

            // TODO: Provide a menu to select the midi device.
            _midiOut = new MidiOut(1);
        }

        public void PlayNote(int noteNumber) {
            var note = _notesOn.FirstOrDefault(n => n.NoteNumber == noteNumber);
            if (note == null) {
                int channel = 1;
                var noteOnEvent = new NoteOnEvent(0, channel, noteNumber, 100, 5000);

                _notesOn.Add(noteOnEvent);
                _midiOut.Send(noteOnEvent.GetAsShortMessage());
            } else {
                _midiOut.Send(note.OffEvent.GetAsShortMessage());
                _midiOut.Send(note.GetAsShortMessage());
            }
        }
        public void StopNote(int noteNumber) {
            var note = _notesOn.FirstOrDefault(n => n.NoteNumber == noteNumber);
            if (note != null) {
                _midiOut.Send(note.OffEvent.GetAsShortMessage());
                _notesOn.Remove(note);
            }
        }
        public void StopAll() {
            foreach (NoteOnEvent n in _notesOn) {
                _midiOut.Send(n.OffEvent.GetAsShortMessage());
            }
            _notesOn.Clear();
        }

        public void Dispose() {
            StopAll();

            _midiOut.Dispose();
        }

        MidiOut _midiOut;

        HashSet<NoteOnEvent> _notesOn = new HashSet<NoteOnEvent>();
    }
}