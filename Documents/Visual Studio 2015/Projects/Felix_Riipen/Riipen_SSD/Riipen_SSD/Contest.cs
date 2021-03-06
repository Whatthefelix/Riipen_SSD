//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Riipen_SSD
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contest()
        {
            this.ContestJudges = new HashSet<ContestJudge>();
            this.Criteria = new HashSet<Criterion>();
            this.Teams = new HashSet<Team>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public bool Published { get; set; }
        public bool PubliclyViewable { get; set; }
        public Nullable<int> WinnerTeamId { get; set; }
        public Nullable<int> SecondTeamId { get; set; }
        public Nullable<int> ThirdTeamId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContestJudge> ContestJudges { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Criterion> Criteria { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Team> Teams { get; set; }
        public virtual Team WinningTeam { get; set; }
        public virtual Team SecondPlaceTeam { get; set; }
        public virtual Team ThirdPlaceTeam { get; set; }
    }
}
