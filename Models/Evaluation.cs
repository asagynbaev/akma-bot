namespace MindMate
{
    public class EvaluationResult
    {
        public EvaluationInfo evaluation { get; set; }

        public class EvaluationInfo
        {
            public double FinalEvaluation { get; set; }
            public int Transactions { get; set; }
            public bool Blacklist { get; set; }
            public double Balance { get; set; }
        }
    }
}