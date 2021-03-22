public class ObservationProgress
{
     public float Time { get; set; }
     public float Progress { get; set; }
     public int Stage { get; set; }

     public ObservationProgress()
     {
          Time = 0f;
          Progress = 0f;
          Stage = 0;
     }
}