using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace humber_http_5226_collaborative_project.Models {
  public class Item {

    //Contains basic details about each Cafe Item. Also contains a reference to 
    //all the Cafe that reference this Item. For example, generic Items such as
    //tea or coffee can be referenced by many different Cafes without being
    //duplicated in the database.

    //A Cafe may have many Items, and an Item may be sold by many Cafes.
    [Key]
    public int ItemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public virtual ICollection<Cafe> CafesWithThisItem { get; set; }
  }
}