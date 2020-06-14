using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using static Choreo.Globals;

namespace Choreo {
    public class CueRow: PropertyChangedNotifier {
        Cue cue;
        public CueRow(Cue cue) {
            this.cue = cue;
            Id = Guid.NewGuid();
        }
        public CueRow(Cue cue, string id) {
            this.cue = cue;
            Id = new Guid(id);
        }
        public Guid Id { get; }
        public int Index => cue.Rows.IndexOf(this);

        public bool[] Motors { get; } = new bool[16];
        public bool[] Groups { get; } = new bool[8];

        public Status Status => !(Motors.Any(m => m) || Groups.Any(g => g));

        public void Validate() => MultiNotify(nameof(Status));

        public bool IsConsistent { 
            get {
                for(int i = 0; i < Motors.Length; i++) if (Motors[i] && (VM.Motors[i].IsGrouped || !VM.Motors[i].UserEnable)) return false;
                return true;
            }
        }

        [Persistent]
        public ushort MotorsBitmap {
            get {
                ushort res = 0;
                for(int i = 0; i < 16; i++) {
                    res <<= 1;
                    if (Motors[15 - i]) res |= 1;
                }
                return res;
            }
            set {
                for (int i = 0; i < 16; i++) {
                    Motors[i] = (value & 1) == 1;
                    value >>= 1;
                }
            }
        }

        [Persistent]
        public ushort GroupsBitmap {
            get {
                byte res = 0;
                for (int i = 0; i < 8; i++) {
                    res <<= 1;
                    if (Groups[7 - i]) res |= 1;
                }
                return res;
            }
            set {
                for (int i = 0; i < 8; i++) {
                    Groups[i] = (value & 1) == 1;
                    value >>= 1;
                }
            }
        }

        double target;
        [DataItem, Persistent]
        public double Target {
            get { return target; }
            set { target = value; Notify(); }
        }

        double velocity;
        [DataItem(mu: "fpm"), Persistent]
        public double Velocity {
            get { return velocity; }
            set { velocity = value; Notify(); }
        }

        double acceleration;
        [DataItem, Persistent]
        public double Acceleration {
            get { return acceleration; }
            set { acceleration = value; Notify(); }
        }

        double deceleration;
        [DataItem, Persistent]
        public double Deceleration {
            get { return deceleration; }
            set { deceleration = value; Notify(); }
        }
    }

    public class Cue : PropertyChangedNotifier {
        public Cue() => Id = Guid.NewGuid();
        public Cue(string id) => Id = Guid.Parse(id);
        public Guid Id { get; }

        public string name;
        [DataItem(title: "Cue Name"), Persistent]
        public string Name {
            get => name == null ? string.Empty : name;
            set { name = value; Notify(); }
        }

        private bool enabled;
        public bool Enabled {
            get { return enabled; }
            set { enabled = value; Notify(); }
        }

        public bool IsValid => Rows.All(row => row.Status == Status.Ok);
        public bool IsConsistent => Rows.All(row => row.IsConsistent);
        public int Index => VM.Cues.IndexOf(this);

        [DataItem(title: "Cue Number")]
        public int Number => Index + 1;
        public void RefreshIndex() { Notify()(nameof(Index), nameof(Number), nameof(Name)); }

        bool show;
        [Persistent]
        public bool Show {
            get { return show; }
            set { show = value; Notify(); }
        }

        TimeSpan duration;
        public TimeSpan Duration {
            get { return duration; }
            set { duration = value; Notify(); }
        }

        public ObservableCollection<CueRow> Rows { get; private set; } = new ObservableCollection<CueRow>();
    }
}
