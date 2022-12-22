namespace postgresql_worker
{
    public class PostgreSQLConfiguration
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string DBname { get; set; }
        public string Password { get; set; }
        public ushort Port { get; set; }
        public ushort ReadDelayTime {get; set;}
        public ushort UpdateDelayTime {get; set;}
        public ushort InsertDelayTime {get; set;}
        public ushort MinPoolSize{get; set;}
        public ushort MaxPoolSize {get; set;}
        public ushort ConnectionLifeTime{get; set;}
    }
}