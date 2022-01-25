using System;
using System.Reflection;

public class SimpleClass
{ }

public class SimpleClassExample
{
    public static void Main()
    {
        printSimpleClassExample();
        spacer();
        instanceTester();
        spacer();
        carExample();
        spacer();
        publicationExample();
        ender();
    }


    public static void spacer()
    {

        Console.WriteLine($"");
        Console.WriteLine($"- - {System.Reflection.MethodBase.GetCurrentMethod().Name} - -");
        Console.WriteLine($"");
    }
    public static void ender()
    {
        Console.WriteLine($"");
        Console.WriteLine($"The end - {System.Reflection.MethodBase.GetCurrentMethod().Name}");

    }
    public static void instanceTester()
    {
        var x = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Console.WriteLine($"In method: {x} ");
        var b = new A.B();
        var c = new C();

        Console.WriteLine(b.GetValue1());
        Console.WriteLine(b.Summation());
        Console.WriteLine(c.GetValue2());
        Console.WriteLine(c.Summation());


    }
    public static void printSimpleClassExample()
    {
        Console.WriteLine($"In method: {System.Reflection.MethodBase.GetCurrentMethod().Name} ");
        SimpleClass sc = new SimpleClass();


        Type t = typeof(SimpleClass);
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                             BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        MemberInfo[] members = t.GetMembers(flags);
        Console.WriteLine($"Type {t.Name} has {members.Length} members: ");
        foreach (var member in members)
        {
            string access = "";
            string stat = "";
            var method = member as MethodBase;
            if (method != null)
            {
                if (method.IsPublic)
                    access = " Public";
                else if (method.IsPrivate)
                    access = " Private";
                else if (method.IsFamily)
                    access = " Protected";
                else if (method.IsAssembly)
                    access = " Internal";
                else if (method.IsFamilyOrAssembly)
                    access = " Protected Internal ";
                if (method.IsStatic)
                    stat = " Static";
            }
            var output = $"{member.Name} ({member.MemberType}): {access}{stat}, Declared by {member.DeclaringType}";
            Console.WriteLine(output);

        }
        Console.WriteLine($"");

        Console.WriteLine($"simple class to string : {sc.ToString()}");



    }
    public static void carExample()
    {
        var packard = new Automobile("Packard", "Custom Eight", 1948);
        Console.WriteLine(packard);
    }

    public static void publicationExample()
    {
        // var publication = new Publication("","",PublicationType.Book);
        var book = new Book("The Tempest", "0971655819", "Shakespeare, William",
                          "Public Domain Press");
        ShowPublicationInfo(book);
        book.Publish(new DateTime(2016, 8, 18));
        ShowPublicationInfo(book);

        var book2 = new Book("The Tempest", "Classic Works Press", "Shakespeare, William");
        Console.Write($"{book.Title} and {book2.Title} are the same publication: " +
              $"{((Publication)book).Equals(book2)}");
    }
    public static void ShowPublicationInfo(Publication pub)
    {
        string pubDate = pub.GetPublicationDate();
        Console.WriteLine($"{pub.Title}, " +
                  $"{(pubDate == "NYP" ? "Not Yet Published" : "published on " + pubDate):d} by {pub.Publisher}");
    }


}
// The example displays the following output:
//	Type SimpleClass has 9 members:
//	ToString (Method):  Public, Declared by System.Object
//	Equals (Method):  Public, Declared by System.Object
//	Equals (Method):  Public Static, Declared by System.Object
//	ReferenceEquals (Method):  Public Static, Declared by System.Object
//	GetHashCode (Method):  Public, Declared by System.Object
//	GetType (Method):  Public, Declared by System.Object
//	Finalize (Method):  Internal, Declared by System.Object
//	MemberwiseClone (Method):  Internal, Declared by System.Object
//	.ctor (Constructor):  Public, Declared by SimpleClass


public sealed class Book : Publication
{
    public Book(string title, string author, string publisher) :
           this(title, String.Empty, author, publisher)
    { }

    public Book(string title, string isbn, string author, string publisher) : base(title, publisher, PublicationType.Book)
    {
        // isbn argument must be a 10- or 13-character numeric string without "-" characters.
        // We could also determine whether the ISBN is valid by comparing its checksum digit
        // with a computed checksum.
        //
        if (!String.IsNullOrEmpty(isbn))
        {
            // Determine if ISBN length is correct.
            if (!(isbn.Length == 10 | isbn.Length == 13))
                throw new ArgumentException("The ISBN must be a 10- or 13-character numeric string.");
            ulong nISBN = 0;
            if (!UInt64.TryParse(isbn, out nISBN))
                throw new ArgumentException("The ISBN can consist of numeric characters only.");
        }
        ISBN = isbn;

        Author = author;
    }

    public string ISBN { get; }

    public string Author { get; }

    public Decimal Price { get; private set; }

    // A three-digit ISO currency symbol.
    public string Currency { get; private set; }

    // Returns the old price, and sets a new price.
    public Decimal SetPrice(Decimal price, string currency)
    {
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "The price cannot be negative.");
        Decimal oldValue = Price;
        Price = price;

        if (currency.Length != 3)
            throw new ArgumentException("The ISO currency symbol is a 3-character string.");
        Currency = currency;

        return oldValue;
    }

    public override bool Equals(object obj)
    {
        Book book = obj as Book;
        if (book == null)
            return false;
        else
            return ISBN == book.ISBN;
    }

    public override int GetHashCode() => ISBN.GetHashCode();

    public override string ToString() => $"{(String.IsNullOrEmpty(Author) ? "" : Author + ", ")}{Title}";
}

public enum PublicationType { Misc, Book, Magazine, Article };
public abstract class Publication
{
    private bool published = false;
    private DateTime datePublished;
    private int totalPages;

    public Publication(string title, string publisher, PublicationType type)
    {
        if (String.IsNullOrWhiteSpace(publisher))
            throw new ArgumentException("The publisher is required.");
        Publisher = publisher;

        if (String.IsNullOrWhiteSpace(title))
            throw new ArgumentException("The title is required.");
        Title = title;

        Type = type;
    }

    public string Publisher { get; }

    public string Title { get; }

    public PublicationType Type { get; }

    public string CopyrightName { get; private set; }

    public int CopyrightDate { get; private set; }

    public int Pages
    {
        get { return totalPages; }
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "The number of pages cannot be zero or negative.");
            totalPages = value;
        }
    }

    public string GetPublicationDate()
    {
        if (!published)
            return "NYP";
        else
            return datePublished.ToString("d");
    }

    public void Publish(DateTime datePublished)
    {
        published = true;
        this.datePublished = datePublished;
    }

    public void Copyright(string copyrightName, int copyrightDate)
    {
        if (String.IsNullOrWhiteSpace(copyrightName))
            throw new ArgumentException("The name of the copyright holder is required.");
        CopyrightName = copyrightName;

        int currentYear = DateTime.Now.Year;
        if (copyrightDate < currentYear - 10 || copyrightDate > currentYear + 2)
            throw new ArgumentOutOfRangeException($"The copyright year must be between {currentYear - 10} and {currentYear + 1}");
        CopyrightDate = copyrightDate;
    }

    public override string ToString() => Title;

}


public class Automobile
{
    public Automobile(string make, string model, int year)
    {
        if (make == null)
            throw new ArgumentNullException(nameof(make), "The make cannot be null.");
        else if (String.IsNullOrWhiteSpace(make))
            throw new ArgumentException("make cannot be an empty string or have space characters only.");
        Make = make;

        if (model == null)
            throw new ArgumentNullException(nameof(model), "The model cannot be null.");
        else if (String.IsNullOrWhiteSpace(model))
            throw new ArgumentException("model cannot be an empty string or have space characters only.");
        Model = model;

        if (year < 1857 || year > DateTime.Now.Year + 2)
            throw new ArgumentException("The year is out of range.");
        Year = year;
    }

    public string Make { get; }

    public string Model { get; }

    public int Year { get; }

    public override string ToString() => $"{Year} {Make} {Model}";
}

public abstract class AbstrClass
{
    public abstract void something();

    public class derivedFromAbstrClass : AbstrClass
    {
        public override void something()
        {

        }
    }
}


public class A
{
    private int value1 = 10;
    public int value2 = 20;
    public int value3 = 30;
    public int value4 = 40;

    //    public int Summation(){
    public virtual int Summation()
    {
        return value1 + value2 + value3;
    }



    public class B : A
    {
        public int GetValue1()
        {
            return this.value1;
        }





    }
}

public class C : A
{
    public int GetValue2()
    {
        return this.value2;
    }

    public override int Summation()
    {
        return value2 + value3 + value4;
    }
}

//! 
// public class AccessExample
// {
//     public static void Main(string[] args)
//     {
//         var b = new A.B();
//         var c = new C();

//         Console.WriteLine(b.GetValue1());
//         Console.WriteLine(b.Summation());
//         Console.WriteLine(c.GetValue2());
//         Console.WriteLine(c.Summation());



//     }
// }
//! 