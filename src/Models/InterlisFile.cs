using System.ComponentModel.DataAnnotations;

namespace ModelRepoBrowser.Models;

public class InterlisFile
{
    [Key]
    public string MD5 { get; set; }

    public string Content { get; set; }

    public ICollection<Model> Models { get; set; }
}
