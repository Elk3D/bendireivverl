public class UIBookDataVO
{
	public Book Book;

	public BookType BookType;

	public string BookLabel;

	public string[] BookPages;

	public UIBookDataVO(Book book, BookType bookType, string bookLabel)
	{
		Book = book;
		BookType = bookType;
		BookLabel = bookLabel;
	}

	public UIBookDataVO(Book book, BookType bookType, params string[] bookPages)
	{
		Book = book;
		BookType = bookType;
		BookPages = bookPages;
	}
}
