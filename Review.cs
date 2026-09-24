public class Review
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}