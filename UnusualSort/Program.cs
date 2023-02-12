// See https://aka.ms/new-console-template for more information
/// <summary>
/// See  https://pvs-studio.com/en/blog/posts/0952/
/// 
/// Here's the problem:
/// 
/// Sort an array of strings such that A1A1, A1A2, ..A1B1.. A10A1 ... A11A1 ... Z12E5
/// 
/// Array members look like A1A1 , A1A2, ...A12B2 ... Z12E5
/// 
/// The expression pattern looks like this: [A-Z][0-9][0-9?][A-E][1-5]
/// 
/// Bonus points given for not writing your own sort. Use libraries.
/// 
/// fedd  06/20/2022, 06:04:57 for Nope
///    put brackets in your pattern, extract the 4 groups out of each string 
///    and then try to sort those groups; try to make it work with any number 
///    of digits or characters in the group as if it was([a-zA-Z]*)([0-9]*)([a-zA-Z]*)([0-9]*)
/// </summary>

static void Main(string[] args)
{
    List<SortableItem> itemList = new List<SortableItem>();

    itemList.Sort()


    Console.WriteLine("Hello, World!");
}

static List<SortableItem> GenerateList(int count)
{
    return new List<SortableItem>();
}


public class SortableItem : String
{
    /*
    internal string s0;
    internal string s1;
    internal string s2;
    internal string s3;
    */

    /*
    public SortableItem(SortableItem si)
    {
        this.s0 = si.s0;
        this.s1 = si.s1;
        this.s2 = si.s2;
        this.s3 = si.s3;
    }

    public SortableItem(string s0, string s1, string s2, string s3)
    {
        this.s0 = s0;
        this.s1 = s1;
        this.s2 = s2;
        this.s3 = s3;
    }

    public string ToString()
    {
        return string.Format($"{this.s0}{this.s1}{this.s2}{this.s3}");
    }
    */

}

public class SIComparer : IComparer<SortableItem>
{
    public int Compare(SortableItem si1, SortableItem si2)
    {
        switch (string.Compare(si1.s0, si2.s0))
        {
            case -1:
                {
                    return -1;
                    break;
                }
            case 0:
                {
                    switch (string.Compare(si1.s1, si2.s1))
                    {
                        case -1:
                            {
                                return -1;
                                break;
                            }
                        case 0:
                            {
                                switch (string.Compare(si1.s2, si2.s2))
                                {
                                    case -1:
                                        {
                                            return -1;
                                            break;
                                        }
                                    case 0:
                                        {
                                            switch (string.Compare(si1.s3, si2.s3))
                                            {
                                                case -1:
                                                    {
                                                        return -1;
                                                        break;
                                                    }
                                                case 0:
                                                    {
                                                        return 0;
                                                    }
                                                case 1:
                                                    {
                                                        return 1;
                                                        break;
                                                    }
                                            }
            case 1:
                                        {
                                            return 1;
                                            break;
                                        }
                                }
            case 1:
                            {
                                return 1;
                                break;
                            }
                    }
            case 1:
                {
                    return 1;
                    break;
                }
        }

        return 0;

    }
}

