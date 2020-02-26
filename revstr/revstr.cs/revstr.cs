using System;


namespace revstr
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class CRevStr
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//
			// TODO: Add code to start application here
			//
			string allArgs;

			if (args.Length > 0) 
			{
				allArgs = args[0];
				for (int i=1;i<args.Length;i++) 
				{
					allArgs += " ";
					allArgs += args[i];
				}

				string result = revstr( allArgs );
				Console.WriteLine( "{0}", result );
			} 
			else 
			{
				System.IO.Stream baseInputStream = Console.OpenStandardInput();
				System.IO.StreamReader inputStream = new System.IO.StreamReader( baseInputStream );

				System.IO.Stream baseOutputStream = Console.OpenStandardOutput();
				System.IO.StreamWriter outputStream = new System.IO.StreamWriter( baseOutputStream );

				allArgs = inputStream.ReadLine();
				while (allArgs != null)
				{
					outputStream.WriteLine( revstr(allArgs) );
					outputStream.Flush();
					allArgs = inputStream.ReadLine();
				}

				/* EOF closes the input stream for us
				inputStream.Close();
				baseInputStream.Close();
				*/
				outputStream.Close();
				baseOutputStream.Close();
			}
			return;
		}
		static private string revstr(string S) 
		{
			int l = S.Length;
			string result = "";
			for (int i=l-1;i>=0;i--) 
			{
				result = result + S[i];
			}
			return result;
		}
	}
}
