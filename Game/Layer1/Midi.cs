using System;
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
            int channel = 1;
            var noteOnEvent = new NoteOnEvent(0, channel, noteNumber, 100, 5000);

            _midiOut.Send(noteOnEvent.GetAsShortMessage());
        }
        public void StopNote(int noteNumber) {
            int channel = 1;
            var noteOnEvent = new NoteOnEvent(0, channel, noteNumber, 100, 5000);

            _midiOut.Send(noteOnEvent.OffEvent.GetAsShortMessage());
        }

        public void Dispose() {
            _midiOut.Dispose();
        }

        MidiOut _midiOut;
    }
}