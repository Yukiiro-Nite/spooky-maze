using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public static int _levelLength = PlayerPrefs.GetInt("levelLength", 5);
    public static bool _hasGuardian = PlayerPrefs.GetInt("hasGuardian", 1) > 0;
    public static bool _isHardcore = PlayerPrefs.GetInt("isHardcore", 1) > 0;
    public static bool _isVR = PlayerPrefs.GetInt("isVR", 1) > 0;

    public static int LevelLength {
        get { return _levelLength; }
        set {
            if(value != _levelLength) {
                _levelLength = value;
                PlayerPrefs.SetInt("levelLength", value);
            }
        }
    }

    public static bool HasGuardian {
        get { return _hasGuardian; }
        set {
            if(_hasGuardian != value) {
                _hasGuardian = value;
                PlayerPrefs.SetInt("hasGuardian", value ? 1 : 0);
            }
        }
    }

    public static bool IsHardcore {
        get { return _isHardcore; }
        set {
            if(_isHardcore != value) {
                _isHardcore = value;
                PlayerPrefs.SetInt("isHardcore", value ? 1 : 0);
            }
        }
    }

    public static bool IsVR {
        get { return _isVR; }
        set {
            if(_isVR != value) {
                _isVR = value;
                PlayerPrefs.SetInt("isVR", value ? 1 : 0);
            }
        }
    }
}
