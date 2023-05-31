namespace API.AccessDTOs
{
    public class AccessResultDto 
    {
        public IEnumerable<AccessListResultDto> Data {get; set;}
        public int Total {get; set;}
        public int Skip {get; set;}
        public int Limit {get; set;}
        public string Sort {get; set;}
    }

    public class AccessListResultDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string MiddleWare { get; set; }

        // public string ApiPath { get; set; }

        public int Status { get; set; }
    }

    public class DeleteAccessResultDto
    {
        public Boolean Status { get; set; }
        public string Message { get; set; }
    }

}