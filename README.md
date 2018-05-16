# FaceId

Uses Microsoft's Face API from Azure Cognitive Services to identify faces in an image.

You need to obtain an API key and URL from Microsoft before you can use this program.

    1. Go to http://portal.azure.com and sign up for an account if you don't already have one.
       Signup is free, but requires a credit card for identify verification.

    2. Once you have your Azure account, click on "Create a Resource" from the portal dashboard,
       and search for the Face API.  Add it to your dashboard.
       
    3. Click the Face API on your dashboard.  Click the Overview tab and look for the Endpoint URL.
       You'll need this information the first time you run the program.
       
    4.  Build and Run the FaceId program.  The first time you run it, it will prompt you for an
        API key and URL.  Get these from the Overview screen in the previous step.  Click Ok.
        The API key and URL will be saved so you never have to enter them again.  You can always
        click the "Configure..." button if you ever need to change them.
        
    5.  You need to add a Person Group first - type the name of your person group in the Person Group
        edit box (e.g. "Family", "Friends", "Famous Poeple", or whatever).  Click the Add button.
        
    6.  Add some people to your Person Group.  Type their name in the Person edit box and click the Add
        button.
        
    7.  Upload some training photos.  Select your Person Group, then select the Person you want to upload
        training photos for.  Click the "Add From File" or "Add from Directory" button and navigate to the file
        or directory containing pictures of the person's face.  For best results, you should upload at least
        10 training photos for each person, from different angles and in different lighting conditions.  The
        more photos you add, and the more varied they are, the better the chance for the program to identify
        that person.  They should be reasonably high quality, and preferably just their face.
        
    8.  Once you've uploaded all your training photos, click the Train button.  Nothing will appear to be happening,
        but that's just because I'm lazy and I havan't implemented anything to show the progress of the training or 
        when it is complete.  This tells the server to start training on the photos you've uploaded.  It shouldn't
        take more than a minute or two, depending on how many training photos you've given it.
        
    9.  Now click the "Open..." button at the bottom right.  Select a photo containing a face you want to identify, 
        or use multiselect by holding the shift or ctrl key.  Your photos will be displayed one at a time with
        the faces labeled for people it can identify.  Unknown people will just be labeled as "Unknown male" or
        "Unknown female", along with an esimated age.  The age estimate is usually pretty accurate for young
	children and older adults, but seems to vary wildly with people in the 18-35 age group.
        
