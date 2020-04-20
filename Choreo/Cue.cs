using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        double target;
        [DataItem]
        public double Target {
            get { return target; }
            set { target = value; OnPropertyChanged(); }
        }

        double velocity;
        [DataItem(mu: "fpm")]
        public double Velocity {
            get { return velocity; }
            set { velocity = value; OnPropertyChanged(); }
        }

        double acceleration;
        [DataItem]
        public double Acceleration {
            get { return acceleration; }
            set { acceleration = value; OnPropertyChanged(); }
        }

        double deceleration;
        [DataItem]
        public double Deceleration {
            get { return deceleration; }
            set { deceleration = value; OnPropertyChanged(); }
        }
    }

    public class Cue : PropertyChangedNotifier {
        public Cue() => Id = Guid.NewGuid();
        public Cue(string id) => Id = Guid.Parse(id);
        public Guid Id { get; }

        public string name;
        [DataItem(title: "Cue Name")]
        public string Name {
            get => name == null ? string.Empty : name;
            set { name = value; OnPropertyChanged(); }
        }

        public int Index => VM.Cues.IndexOf(this);

        [DataItem(title: "Cue Number")]
        public int Number => Index + 1;
        public void RefreshIndex() { OnPropertyChanged("Index"); OnPropertyChanged("Number"); OnPropertyChanged("Name"); }

        bool show;
        public bool Show {
            get { return show; }
            set { show = value; OnPropertyChanged(); }
        }

        TimeSpan duration;
        public TimeSpan Duration {
            get { return duration; }
            set { duration = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CueRow> Rows { get; private set; } = new ObservableCollection<CueRow>();
    }
}
