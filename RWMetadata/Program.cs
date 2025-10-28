// See https://aka.ms/new-console-template for more information


using System;
using TagLib;

class RWMetadata
{
    static void Main(string[] args)
    {
        try
        {
            // Specify the path to your MP3 file
            string filePath = @"C:\Path\To\Your\Music.mp3";

            // Read metadata
            ReadMetadata(filePath);

            // Write new metadata
            WriteMetadata(filePath, "New Title", "New Artist", "New Album", 2023);

            // Read metadata again to verify changes
            ReadMetadata(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void ReadMetadata(string filePath)
    {
        // Load the MP3 file
        using (var file = TagLib.File.Create(filePath))
        {
            // Access the ID3 tags
            var tag = file.Tag;

            // Read metadata
            Console.WriteLine("Current Metadata:");
            Console.WriteLine($"Title: {tag.Title}");
            Console.WriteLine($"Artist: {tag.FirstPerformer}");
            Console.WriteLine($"Album: {tag.Album}");
            Console.WriteLine($"Year: {tag.Year}");
            Console.WriteLine($"Genre: {tag.FirstGenre}");
            Console.WriteLine($"Track: {tag.Track}");
            Console.WriteLine($"Duration: {file.Properties.Duration}");
            Console.WriteLine();
        }
    }

    static void WriteMetadata(string filePath, string newTitle, string newArtist, string newAlbum, uint newYear)
    {
        // Load the MP3 file
        using (var file = TagLib.File.Create(filePath))
        {
            // Access the ID3 tags
            var tag = file.Tag;

            // Update metadata
            tag.Title = newTitle;
            tag.Performers = new[] { newArtist };
            tag.Album = newAlbum;
            tag.Year = newYear;

            // Save changes to the file
            file.Save();
            Console.WriteLine("Metadata updated successfully.");
        }
    }
}
