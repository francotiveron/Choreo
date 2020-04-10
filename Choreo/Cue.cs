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
        public List<int> Motors { get; private set; }
        public List<int> Groups { get; private set; }

        double target;
        [DataItem(edit:true)]
        public double Target {
            get { return target; }
            set { target = value; OnPropertyChanged(); }
        }

        double velocity;
        [DataItem(edit: true)]
        public double Velocity {
            get { return velocity; }
            set { velocity = value; OnPropertyChanged(); }
        }

        double acceleration;
        [DataItem(edit: true)]
        public double Acceleration {
            get { return acceleration; }
            set { acceleration = value; OnPropertyChanged(); }
        }

        double deceleration;
        [DataItem(edit: true)]
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
        [DataItem(title: "Cue Name", edit: true)]
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
