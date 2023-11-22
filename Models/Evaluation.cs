namespace MindMate
{
    public class EvaluationResult
    {
        public FinalEvaluationData FinalEvaluation { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }

        public class FinalEvaluationData
        {
            public double FinalEvaluation { get; set; }
            public int Transactions { get; set; }
            public bool Blacklist { get; set; }
            public double Balance { get; set; }
            public DateTime First_Transaction { get; set; }
            public DateTime Last_Transaction { get; set; }
        }
    }
}