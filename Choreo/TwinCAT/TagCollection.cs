using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static Choreo.Globals;

namespace Choreo.TwinCAT {
    public class TagCollection: IEnumerable<ITag> {
        static TagCollection instance;
        public static TagCollection Instance {
            get {
                if (instance == null) instance = new TagCollection();
                return instance;
            }
        }

        Dictionary<string, ITag> tagsByPath = new Dictionary<string, ITag>();
        TagCollection() {
            var tags = tagDefinitions.SelectMany(def => def.Build()).ToList();
            tagsByPath = tags.ToDictionary(tag => tag.Path);
        }

        public IEnumerator<ITag> GetEnumerator() => tagsByPath.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => tagsByPath.Values.GetEnumerator();

        public ITag this[string path] => tagsByPath[path];

        #region Tag Defintions
        static ITagDef[] tagDefinitions {
            get {
                return new ITagDef[] {
                    new SimpleTagDef { Path = "Move_Active", Property = "MoveActive" }
                    , new SimpleTagDef { Path = "Cue_loaded", Property = "CueLoaded" }

                    , new MotorTagDef { Path = "Act_Pos" , Property = "Position" }
                    , new MotorTagDef { Path = "Load" , Property = "Load" }
                    , new MotorTagDef { Path = "Min_Load", Property = "MinLoad" }
                    , new MotorTagDef { Path = "Max_Load", Property = "MaxLoad" }
                    , new MotorTagDef { Path = "Move_Val", Property = "MoveVal" }
                    , new MotorTagDef { Path = "Accel", Property = "Accel" }
                    , new MotorTagDef { Path = "Decel", Property = "Decel" }
                    , new MotorTagDef { Path = "Velocity", Property = "Velocity" }
                    , new MotorTagDef { Path = "MA_Enable", Property = "MAEnable" }
                    , new MotorTagDef { Path = "MR_Enable", Property = "MREnable" }
                    , new MotorTagDef { Path = "Jog_Up_Enable", Property = "JogUpEnable" }
                    , new MotorTagDef { Path = "Jog_Dn_Enable", Property = "JogDnEnable" }
                    , new MotorTagDef { Path = "Fwd_Lim", Property = "FwdLim" }
                    , new MotorTagDef { Path = "Rev_Lim", Property = "RevLim" }
                    , new MotorTagDef { Path = "Fwd_Ult_Lim", Property = "FwdUltLim" }
                    , new MotorTagDef { Path = "Rev_Ult_Lim", Property = "RevUltLim" }
                    , new MotorTagDef { Path = "Over_Load", Property = "OverLoad" }
                    , new MotorTagDef { Path = "Under_Load", Property = "UnderLoad" }
                    , new MotorTagDef { Path = "Drive_Status", Property = "DriveStatus" }
                    , new MotorTagDef { Path = "Move_Complete", Property = "MoveComplete" }
                    , new MotorTagDef { Path = "User_Enable", Property = "UserEnable" }
                    , new MotorTagDef { Path = "ES_Status", Property = "ESStatus" }
                    , new MotorTagDef { Path = "Active", Property = "Active" }
                    , new MotorTagDef { Path = "Present", Property = "Present" }
                    , new MotorTagDef { Path = "Calibration_Value", Property = "CalibrationValue" }
                    , new MotorTagDef { Path = "Calibration_Save", Property = "CalibrationSave" }
                };
            }
        }

        interface ITagDef { IEnumerable<ITag> Build(); }
        abstract class TagDefBase : ITagDef {
            public string Path;
            public string Property;
            protected Action<object> PropertySetter(object obj) {
                var pi = obj.GetType().GetProperty(Property);
                return (value) => pi.SetValue(obj, value);
            }
            public abstract IEnumerable<ITag> Build();
        }

        class SimpleTagDef: TagDefBase {
            public override IEnumerable<ITag> Build() {
                yield return new Tag(Path, PropertySetter(VM)); 
            }
        }

        class MotorTagDef : SimpleTagDef {
            public override IEnumerable<ITag> Build() {
                for (int i = 0; i < VM.Motors.Count; i++) {
                    var motor = VM.Motors[i];
                    yield return new Tag($"Axis_{motor.Number:00}_{Path}", PropertySetter(motor));
                }
            }
        }
        class GroupTagDef : SimpleTagDef { 
        
        }

        #endregion
    }

}
