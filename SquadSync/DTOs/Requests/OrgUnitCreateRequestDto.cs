namespace SquadSync.DTOs.Requests
{
    public class OrgUnitCreateRequestDto
    {
        public string OrgUnitName { get; set; }
        public string OrgUnitDescription { get; set; }
        public Guid OwnerUserGuid { get; set; }
        public string OrgUnitStatus { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
