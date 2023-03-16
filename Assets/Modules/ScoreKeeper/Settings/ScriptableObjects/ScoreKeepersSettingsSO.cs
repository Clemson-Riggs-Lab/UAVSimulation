using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;
using static HelperScripts.Enums;
using static HelperScripts.Enums.InputRecordsSource;

namespace Modules.ScoreKeeper.Settings.ScriptableObjects
{
   [CreateAssetMenu(fileName = "ScoreKeepersSettings", menuName = "Settings/ScoreKeepersSettings")]
   public class ScoreKeepersSettingsSO : ScriptableObject
   {
      
      [Header("Score Keeping Settings")]
      public bool keepScoreOfReroutingFalsePositives = true;
      public bool keepScoreOfReroutingFalseNegatives = true;
      public bool keepScoreOfReroutingTruePositives = true;
      public bool keepScoreOfReroutingTrueNegatives = true;
      
      public bool keepScoreOfTargetDetectionFalsePositives = true;
      public bool keepScoreOfTargetDetectionFalseNegatives = true;
      public bool keepScoreOfTargetDetectionTruePositives = true;
      public bool keepScoreOfTargetDetectionTrueNegatives = true;
      

      [Header("Score Calculation Settings")]
      
      public float reroutingFalsePositiveScore = -1; //rerouted when uav was not headed to nfz
      public float reroutingFalseNegativeScore = -2; //not rerouted when uav was headed to nfz
      public float reroutingTruePositiveScore = +2; //rerouted when uav was headed to nfz
      public float reroutingTrueNegativeScore = +1; //not rerouted when uav was not headed to nfz

      public bool awardAndPenalizeForIntermediaryRerouting = true;

      public float targetDetectionFalsePositiveScore = -1;
      public float targetDetectionFalseNegativeScore = -2;
      public float targetDetectionTruePositiveScore = +2;
      public float targetDetectionTrueNegativeScore = +1;
      
      public bool awardAndPenalizeForIntermediaryTargetDetection = true;
      
      
      [Header("Score Logging Settings")]
      public bool logScoreResults = true;
      public bool logScoringCriteria = true;
      public bool showScorePanel=false;
   }
}
