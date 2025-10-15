namespace DCAssignment2A.Models
{
    public class ProfilePicReader
    {
        //setting the default byte[] of userprofile
        //the output on swagger's json is extremely massive though.
        public static byte[] GetDefaultPFP()
        {
            string filepath = Path.Combine(AppContext.BaseDirectory, "Resources", "default.jpg");
            return File.ReadAllBytes(filepath);
        }

        public static byte[] GetPFP(string pfp)
        {
            string filepath = Path.Combine(AppContext.BaseDirectory, "Resources", pfp);
            return File.ReadAllBytes(filepath);
        }
    }
}
