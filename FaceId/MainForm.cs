#define USE_OPENCV

using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

// If we're using OpenCV, all of the async methods will flag this warning since we don't actually do anything asynchronously
// in OpenCV mode.  Just disable the warning.
#if USE_OPENCV
#pragma warning disable CS1998
#endif

namespace FaceId
{
    public partial class MainForm : Form
    {
        private IFaceServiceClient faceServiceClient;
        PersonGroup[] personGroups;
        Person[] persons;
        string apiKey;
        string url;

        public MainForm()
        {
            InitializeComponent();
        }


#if USE_OPENCV
        CascadeClassifier classifier = null;
        OpenCvSharp.Face.FaceRecognizer faceRecognizer = null;

        private OpenCvSharp.Rect [] DetectFacesInImage(Mat img, HaarDetectionType detectionType, OpenCvSharp.Size minSize)
        {
            if (classifier == null)
            {
                classifier = new CascadeClassifier(@"../../../haarcascade_frontalface_alt.xml");
//                var nestedCascade = new CascadeClassifier(@"..\..\Data\haarcascade_eye_tree_eyeglasses.xml");
            }

            OpenCvSharp.Rect [] faces = classifier.DetectMultiScale(
                 image: img,
                 scaleFactor: 1.1,
                 minNeighbors: 5,
//                 flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
//                 flags: HaarDetectionType.FindBiggestObject | HaarDetectionType.ScaleImage,
                 flags: detectionType | HaarDetectionType.ScaleImage,
                 minSize: minSize);
            return faces;
        }
#endif

        private async void Form1_Load(object sender, EventArgs e)
        {
#if USE_OPENCV
            faceServiceClient = null;
#else
            try
            {
                using (StreamReader configFile = new StreamReader("./FaceAPI.config"))
                {
                    apiKey = configFile.ReadLine();
                    url = configFile.ReadLine();
                    configFile.Close();
                }
            }
            catch (FileNotFoundException)
            {
                ConfigForm configForm = new ConfigForm();
                configForm.configRequired = true;
                DialogResult result = configForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    apiKey = configForm.GetApiKey();
                    url = configForm.GetURL();

                    if (apiKey == "" || url == "")
                    {
                        return;
                    }
                    using (StreamWriter configFile = new StreamWriter("./FaceApi.config"))
                    {
                        configFile.WriteLine(apiKey);
                        configFile.WriteLine(url);
                        configFile.Close();
                    }
                }
            }

            faceServiceClient = new FaceServiceClient(apiKey, url);
#endif
            personGroups = await GetPersonGroups();
            foreach (PersonGroup personGroup in personGroups)
            {
                personGroupSelector.Items.Add(personGroup.Name);
            }

            personSelector.Enabled = false;
            addFromFileButton.Enabled = false;
            deletePersonGroupButton.Enabled = false;
            deletePersonButton.Enabled = false;
            deleteFacesButton.Enabled = false;
            addPersonGroupButton.Enabled = false;
            addPersonButton.Enabled = false;
            identifyButton.Enabled = false;
            nextButton.Enabled = false;
            prevButton.Enabled = false;
            trainButton.Enabled = false;
        }


        protected void ResizeImage(Stream fromStream, Stream toStream)
        {
            var image = Image.FromStream(fromStream);

            if (image.Width <= 1200)
            {
                fromStream.Seek(0, SeekOrigin.Begin);
                fromStream.CopyTo(toStream);
                image.Dispose();
                return;
            }
            var scaleFactor = 1200 / (double)image.Width;
            var newWidth = 1200;
            var newHeight = (int)(image.Height * scaleFactor);
            var thumbnailBitmap = new Bitmap(newWidth, newHeight);

            var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(image, imageRectangle);

            thumbnailBitmap.Save(toStream, image.RawFormat);

            thumbnailGraph.Dispose();
            thumbnailBitmap.Dispose();
            image.Dispose();

            toStream.Seek(0, SeekOrigin.Begin);
        }



        private async Task<PersonGroup []> GetPersonGroups()
        {
#if USE_OPENCV
            // Create the Data/ directory if it doesn't already exist
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("./Data");
            }

            List<PersonGroup>pList = new List<PersonGroup>();
            try
            {
                string [] directories = Directory.GetDirectories("Data");
                foreach (string directory in directories)
                {
                    PersonGroup p = new PersonGroup();
                    p.Name = directory.ToString().Split('\\')[1];
                    p.PersonGroupId = directory.ToString();
                    pList.Add(p);
                }
            }
            catch (Exception)
            {

            }
            return pList.ToArray();
#else
            var personGroups = await faceServiceClient.ListPersonGroupsAsync();
            return personGroups.ToArray();
#endif
        }

        private async Task<Person []> GetPersonsInPersonGroup(string personGroupId)
        {
#if USE_OPENCV
            List<Person> pList = new List<Person>();
            foreach (PersonGroup group in personGroups)
            {
                if (group.PersonGroupId == personGroupId)
                {
                    try
                    {
                        using (StreamReader s = new StreamReader(group.PersonGroupId + "/Persons.dat"))
                        {
                            while (!s.EndOfStream)
                            {
                                string line = s.ReadLine();
                                string[] parts = line.Split('|');
                                Person p = new Person();
                                p.Name = parts[0];
                                p.UserData = parts[1];
                                pList.Add(p);
                            }
                            s.Close();
                        }
                    }
                    catch(Exception)
                    {

                    }
                }
            }
            return pList.ToArray();
#else
            var persons = await faceServiceClient.ListPersonsInPersonGroupAsync(personGroupId);
            return persons.ToArray();
#endif
        }

        private async void personGroupSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            personSelector.Enabled = true;
            deletePersonGroupButton.Enabled = true;
            addPersonGroupButton.Enabled = false;
            identifyButton.Enabled = true;
            trainButton.Enabled = true;
            deletePersonButton.Enabled = false;

            PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
            persons = await GetPersonsInPersonGroup(selectedPersonGroup.PersonGroupId);
            personSelector.Items.Clear();
            foreach (Person person in persons)
            {
                personSelector.Items.Add(person.Name);
            }
            personSelector.Text = "";
        }

        private void personSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            addFromFileButton.Enabled = true;
            deletePersonButton.Enabled = true;
            deleteFacesButton.Enabled = true;
            addPersonButton.Enabled = false;
        }

        private async void faceUploadButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                // Send image(s) to server
                imgInfoBox.Text = "";
                int count = 0;
                foreach (string filename in openFileDialog.FileNames)
                {
#if USE_OPENCV
                    count++;
                    PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
                    Person selectedPerson = persons[personSelector.SelectedIndex];
                    Mat image = new Mat(filename, ImreadModes.GrayScale);
                    OpenCvSharp.Rect[] faces = DetectFacesInImage(image, HaarDetectionType.DoRoughSearch, new OpenCvSharp.Size(300,300));
                    if (faces.Length > 1)
                    {
                        MessageBox.Show(filename + ": Too many faces detected in this image", "Error");
                        foreach (OpenCvSharp.Rect rect in faces)
                        {
                            Mat errCrop = new Mat(image, rect);
                            Mat errResizedImage = new Mat();
                            Cv2.Resize(errCrop, errResizedImage, new OpenCvSharp.Size(400, 400));
                            Cv2.ImShow("Detected Face", errResizedImage);
                            Cv2.WaitKey(0);
                            Cv2.DestroyWindow("Detected Face");
                        }
                        return;
                    }
                    if (faces.Length < 1)
                    {
                        MessageBox.Show(filename + ": No faces detected in this image", "Error");
                    }
                    else
                    {
                        Mat crop = new Mat(image, faces[0]);
                        Mat resizedImage = new Mat();
                        Cv2.Resize(crop, resizedImage, new OpenCvSharp.Size(400, 400));
                        Cv2.ImShow("Added Image", resizedImage);
                        Cv2.WaitKey(0);
                        Cv2.DestroyWindow("Added Image");

                        using (StreamWriter s = new StreamWriter(selectedPersonGroup.PersonGroupId + "/TrainData.dat", true))
                        {
                            s.WriteLine(filename + "|" + selectedPerson.UserData);
                            s.Close();
                        }
                    }
#else
                    System.IO.MemoryStream imageMemoryStream = new MemoryStream();
                    System.IO.FileStream imageFileStream = new FileStream(filename, FileMode.Open);
                    ResizeImage(imageFileStream, imageMemoryStream);
                    imageFileStream.Close();

                    PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
                    Person selectedPerson = persons[personSelector.SelectedIndex];
                    try
                    {
                        await faceServiceClient.AddPersonFaceInPersonGroupAsync(selectedPersonGroup.PersonGroupId, selectedPerson.PersonId, imageMemoryStream);
                        imgInfoBox.Text += filename + ": added successfully\r\n";
                    }
                    catch (FaceAPIException ex)
                    {
                        imgInfoBox.Text += filename + ": " + ex.ErrorMessage + "(" + ex.ErrorCode + ")\r\n";

                        // This shouldn't happen because of the throttling that happens below, but handle it just in case
                        if (ex.ErrorCode == "RateLimitExceeded")
                            System.Threading.Thread.Sleep(60000);
                    }

                    // We're only allowed 20 API calls per minute.  If we're up to 20, wait a minute before proceeding.
                    if (++count > 19)
                    {
                        imgInfoBox.Text += "Reached rate limit - waiting 60 seconds\r\n";
                        System.Threading.Thread.Sleep(60000);
                        count = 0;
                    }
#endif
                }
            }
        }

        private async void deleteFacesButton_Click(object sender, EventArgs e)
        {
            PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
            Person selectedPerson = persons[personSelector.SelectedIndex];

            FaceListMetadata[] faceListMetaData = await faceServiceClient.ListFaceListsAsync();
            foreach (FaceListMetadata meta in faceListMetaData)
            {
                if (meta.Name == selectedPerson.Name)
                {
                    FaceList faceList = await faceServiceClient.GetFaceListAsync(meta.FaceListId);
                    PersistedFace[] persistedFaces = faceList.PersistedFaces;
                    foreach (PersistedFace f in persistedFaces)
                    {
                        await faceServiceClient.DeleteFaceFromFaceListAsync(faceList.FaceListId, f.PersistedFaceId);
                    }
                }
            }

        }

        private async void deletePersonButton_Click(object sender, EventArgs e)
        {
            PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
            Person selectedPerson = persons[personSelector.SelectedIndex];

            await faceServiceClient.DeletePersonFromPersonGroupAsync(selectedPersonGroup.PersonGroupId, selectedPerson.PersonId);

            persons = await GetPersonsInPersonGroup(selectedPersonGroup.PersonGroupId);
            personSelector.Items.Clear();
            foreach (Person person in persons)
            {
                personSelector.Items.Add(person.Name);
            }
            personSelector.SelectedIndex = 0;
            personSelector.Text = "";
        }

        private async void addPersonGroupButton_Click(object sender, EventArgs e)
        {
            if (personGroupSelector.Text.Length != 0)
            {
                if (personGroupSelector.Text.Length > 0)
                {
#if USE_OPENCV
                    if (!Directory.Exists("Data/" + personGroupSelector.Text))
                    {
                        Directory.CreateDirectory("Data/" + personGroupSelector.Text);
                    }
#else
                    await faceServiceClient.CreatePersonGroupAsync(personGroupSelector.Text.ToLower(), personGroupSelector.Text);
#endif
                    personGroups = await GetPersonGroups();
                    personSelector.SelectedIndex = -1;
                    if (personGroupSelector.Items.Count > 0)
                        personGroupSelector.Text = personGroupSelector.Items[0].ToString();
                    else
                        personGroupSelector.Text = "";

                    personGroupSelector.Items.Clear();
                    foreach (PersonGroup personGroup in personGroups)
                    {
                        personGroupSelector.Items.Add(personGroup.Name);
                    }
                    personSelector.Enabled = true;
                    persons = null;
                    personSelector.Items.Clear();
                }
            }
                    addPersonGroupButton.Enabled = false;
        }

        private void personGroupSelector_KeyPress(object sender, KeyPressEventArgs e)
        {
            personSelector.Text = "";
            personSelector.SelectedIndex = -1;
            addPersonGroupButton.Enabled = true;
            deletePersonGroupButton.Enabled = false;
            addPersonButton.Enabled = false;
            deletePersonButton.Enabled = false;
            addFromFileButton.Enabled = false;
            deleteFacesButton.Enabled = false;
            personSelector.Enabled = false;
            trainButton.Enabled = false;
        }

        private void personSelector_KeyPress(object sender, KeyPressEventArgs e)
        {
            addPersonButton.Enabled = true;
            deletePersonButton.Enabled = false;
            addFromFileButton.Enabled = false;
            deleteFacesButton.Enabled = false;
        }

        private async void addPersonButton_Click(object sender, EventArgs e)
        {
            PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
            if (personSelector.Text.Length != 0)
            {
                if (personSelector.Text.Length > 0)
                {
#if USE_OPENCV
                    int maxPersonNum = 0;
                    try
                    {
                        using (StreamReader r = new StreamReader(selectedPersonGroup.PersonGroupId + "/Persons.dat"))
                        {
                            while (!r.EndOfStream)
                            {
                                string line = r.ReadLine();
                                string[] parts = line.Split('|');
                                int personNum = Convert.ToInt32(parts[1]);
                                if (personNum > maxPersonNum)
                                    maxPersonNum = personNum;
                            }
                            r.Close();
                            maxPersonNum++;
                        }
                    }
                    catch(Exception)
                    {

                    }
                    try
                    { 
                        using (StreamWriter s = new StreamWriter(selectedPersonGroup.PersonGroupId + "/Persons.dat", true))
                        {
                            s.WriteLine(personSelector.Text + "|" + maxPersonNum);
                            s.Close();
                        }
                    }
                    catch (Exception)
                    {

                    }
#else
                    await faceServiceClient.CreatePersonInPersonGroupAsync(selectedPersonGroup.PersonGroupId, personSelector.Text);
#endif
                    persons = await GetPersonsInPersonGroup(selectedPersonGroup.PersonGroupId);
                    personSelector.SelectedIndex = -1;
                    personSelector.Items.Clear();
                    foreach (Person person in persons)
                    {
                        personSelector.Items.Add(person.Name);
                    }
                }
            }
            deletePersonButton.Enabled = true;
            addPersonButton.Enabled = false;
        }

        private async void deletePersonGroupButton_Click(object sender, EventArgs e)
        {
            PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
            await faceServiceClient.DeletePersonGroupAsync(selectedPersonGroup.PersonGroupId);

            personGroups = await GetPersonGroups();
            personGroupSelector.Items.Clear();
            foreach (PersonGroup personGroup in personGroups)
            {
                personGroupSelector.Items.Add(personGroup.Name);
            }
            personGroupSelector.SelectedIndex = 0;
            personGroupSelector.Text = "";
            deletePersonGroupButton.Enabled = false;
            deletePersonButton.Enabled = false;
            personSelector.SelectedIndex = -1;
            personSelector.Text = "";
        }

        private async void trainButton_Click(object sender, EventArgs e)
        {
            PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
#if USE_OPENCV
            List<Mat> images = new List<Mat>();
            List<int> labels = new List<int>();
            using (StreamReader r = new StreamReader(selectedPersonGroup.PersonGroupId + "/TrainData.dat"))
            {
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();
                    string[] parts = line.Split('|');

                    Mat image = new Mat(parts[0], ImreadModes.GrayScale);
                    OpenCvSharp.Rect[] faces = DetectFacesInImage(image, HaarDetectionType.DoRoughSearch, new OpenCvSharp.Size(300,300));
                    if (faces.Length == 1)
                    {
                        Mat crop = new Mat(image, faces[0]);
                        Mat resizedImage = new Mat();
                        Cv2.Resize(crop, resizedImage, new OpenCvSharp.Size(400, 400));
                        images.Add(resizedImage);
                        labels.Add(Convert.ToInt32(parts[1]));
                    }
                }
                faceRecognizer = OpenCvSharp.Face.FisherFaceRecognizer.Create();
                faceRecognizer.Train(images, labels);
                faceRecognizer.Write(selectedPersonGroup.PersonGroupId + "/Recognizer.dat");
            }
#else
            await faceServiceClient.TrainPersonGroupAsync(selectedPersonGroup.PersonGroupId);
#endif
        }

        Dictionary<Guid, Face> faceDict = new Dictionary<Guid, Face>();
        ArrayList faceList = new ArrayList();
        ArrayList faceNames = new ArrayList();
        string [] files;
        int currFileIdx;

        private void identifyButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
#if USE_OPENCV
                PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
                faceRecognizer = OpenCvSharp.Face.FisherFaceRecognizer.Create();
                faceRecognizer.Read(selectedPersonGroup.PersonGroupId + "/Recognizer.dat");
#endif
                files = openFileDialog.FileNames;
                currFileIdx = 0;

                OpenAndIdentifyImage(files[currFileIdx]);

                EnableNextAndPrevButtons();
            }
        }

        private async void OpenAndIdentifyImage(string filename)
        {
            faceList.Clear();
            faceNames.Clear();
            faceDict.Clear();

            System.IO.MemoryStream imageMemoryStream = new MemoryStream();
            System.IO.FileStream imageFileStream = new FileStream(filename, FileMode.Open);
            ResizeImage(imageFileStream, imageMemoryStream);
            imageMemoryStream.Seek(0, SeekOrigin.Begin);
            imageFileStream.Close();

            imgBox.Image = Image.FromStream(imageMemoryStream);
            imageMemoryStream.Seek(0, SeekOrigin.Begin);
            imgBox.SizeMode = PictureBoxSizeMode.Zoom;
            picLabel.Text = String.Format("{0}/{1}", currFileIdx + 1, files.Length);

            PersonGroup selectedPersonGroup = personGroups[personGroupSelector.SelectedIndex];
#if USE_OPENCV
            Mat image = new Mat(filename, ImreadModes.GrayScale);
            OpenCvSharp.Rect[] faces = DetectFacesInImage(image, HaarDetectionType.DoRoughSearch, new OpenCvSharp.Size(75,75));
            if (faces.Length == 0)
            {
                imgInfoBox.Text = "No faces detected in this image.";
                return;
            }
            imgInfoBox.Text = "";
            int faceNum = 0;
            foreach (OpenCvSharp.Rect rect in faces)
            {
                faceNum++;
                Mat crop = new Mat(image, rect);
                Mat resizedImage = new Mat();
                Cv2.Resize(crop, resizedImage, new OpenCvSharp.Size(400, 400));
                int result = faceRecognizer.Predict(resizedImage);
                if (result > 0)
                {
                    Face face = new Face();
                    face.FaceRectangle = new FaceRectangle();
                    face.FaceRectangle.Left = rect.Left;
                    face.FaceRectangle.Top = rect.Top;
                    face.FaceRectangle.Width = rect.Width;
                    face.FaceRectangle.Height = rect.Height;
                    faceList.Add(face);
                    faceNames.Add(persons[result - 1].Name);
                    imgInfoBox.Text += String.Format("Face {0}: Identified as {1}\r\n", faceNum, persons[result - 1].Name);
                }
                else
                {
                    imgInfoBox.Text += "Unknown";
                }
            }
            imgBox.ImageLocation = filename;
            imgBox.Refresh();

#else
            try
            {
                FaceAttributeType[] faceAttributes = { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Emotion, FaceAttributeType.Accessories };
                Face[] faces = await faceServiceClient.DetectAsync(imageMemoryStream, true, false, faceAttributes);
                if (faces.Length == 0)
                {
                    imgInfoBox.Text = "No faces detected in this image.";
                    return;
                }

                var faceIds = faces.Select(face => face.FaceId).ToArray();

                foreach (Face face in faces)
                {
                    faceDict.Add(face.FaceId, face);
                }
                imgInfoBox.Text = "";

                IdentifyResult[] results = await faceServiceClient.IdentifyAsync(selectedPersonGroup.PersonGroupId, faceIds);
                foreach (var identifyResult in results)
                {
                    imgInfoBox.Text += String.Format("Face {0}: ", identifyResult.FaceId);
                    if (identifyResult.Candidates.Length == 0)
                    {
                        imgInfoBox.Text += "Unknown\r\n";
                        Face f = faceDict[identifyResult.FaceId];
                        faceList.Add(f);
                        faceNames.Add("Unknown");
                    }
                    else
                    {
                        // Get top 1 among all candidates returned
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var person = await faceServiceClient.GetPersonInPersonGroupAsync(selectedPersonGroup.PersonGroupId, candidateId);
                        imgInfoBox.Text += String.Format("Identified as {0}\r\n", person.Name);

                        Face f = faceDict[identifyResult.FaceId];
                        faceList.Add(f);
                        faceNames.Add(person.Name);
                    }
                }
                imgBox.Refresh();
            }
            catch (Exception ex)
            {
                imgInfoBox.Text = String.Format("Error returned from Face API: {0}", ex.Message);
            }
#endif
        }

        private void imgBox_Click(object sender, EventArgs e)
        {

        }

        private void imgBox_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Point p = imgBox.PointToClient(Cursor.Position);

            Rectangle scaledRect;

            for (int n = 0; n < faceList.Count; n++)
            {
                Face face = (Face)faceList[n];

                int w_i = imgBox.Image.Width;
                int h_i = imgBox.Image.Height;
                int w_c = imgBox.Width;
                int h_c = imgBox.Height;
                float imageRatio = w_i / (float)h_i; // image W:H ratio
                float containerRatio = w_c / (float)h_c; // container W:H ratio

                int left, top, width, height;
                if (imageRatio >= containerRatio)
                {
                    // horizontal image
                    float scaleFactor = w_c / (float)w_i;
                    float scaledHeight = h_i * scaleFactor;

                    // calculate gap between top of container and top of image
                    float filler = Math.Abs(h_c - scaledHeight) / 2;

                    FaceRectangle r = face.FaceRectangle;
                    left = (int)((float)r.Left * scaleFactor);
                    top = (int)((float)(r.Top * scaleFactor) + filler);
                    width = (int)((float)r.Width * scaleFactor);
                    height = (int)((float)r.Height * scaleFactor);

                    scaledRect = new Rectangle(left, top, width, height);
                }
                else
                {
                    // vertical image
                    float scaleFactor = h_c / (float)h_i;
                    float scaledWidth = w_i * scaleFactor;

                    // calculate gap between top of container and top of image
                    float filler = Math.Abs(w_c - scaledWidth) / 2;

                    FaceRectangle r = face.FaceRectangle;
                    left = (int)((float)(r.Left * scaleFactor) + filler);
                    top = (int)((float)r.Top * scaleFactor);
                    width = (int)((float)r.Width * scaleFactor);
                    height = (int)((float)r.Height * scaleFactor);

                    scaledRect = new Rectangle(left, top, width, height);
                }
                Color penColor = Color.Green;
                Pen pen = new Pen(penColor, 2f);

                e.Graphics.DrawRectangle(pen, scaledRect);

                string faceAttributes = null;
                if (face.FaceAttributes != null)
                {
                    faceAttributes = String.Format("Age {0}|{1}", Math.Round(face.FaceAttributes.Age, MidpointRounding.AwayFromZero), face.FaceAttributes.Gender);
                    if (face.FaceAttributes.Glasses != Glasses.NoGlasses)
                        faceAttributes += "|Glasses";

                    if (face.FaceAttributes.Accessories != null)
                    {
                        foreach (Accessory a in face.FaceAttributes.Accessories)
                        {
                            if (a.Type == AccessoryType.Headwear)
                                faceAttributes += "|Hat";
                        }
                    }
                }

                string name = (string)faceNames[n];
                int textPos = -17;
                Font font = new Font(FontFamily.GenericSansSerif, 10);
                SolidBrush bkgBrush = new SolidBrush(penColor);
                SolidBrush textBrush = new SolidBrush(Color.White);
                if (name == "Unknown" && faceAttributes != null)
                {
                    string[] parts = faceAttributes.Split('|');
                    for (int m = 0; m < parts.Length; m++)
                    {
                        string s;
                        if (m == 1)
                            s = String.Format("{0} {1}", name, parts[m]);
                        else
                            s = parts[m];
                        SizeF size = e.Graphics.MeasureString(s, font);
                        e.Graphics.FillRectangle(bkgBrush, (float)left, (float)top + textPos, size.Width, size.Height);
                        e.Graphics.DrawString(s, font, textBrush, (float)(left), (float)(top + textPos));
                        textPos -= 15;
                    }
                }
                else
                {
                    SizeF size = e.Graphics.MeasureString(name, font);
                    e.Graphics.FillRectangle(bkgBrush, (float)left, (float)top + textPos, size.Width, size.Height);
                    e.Graphics.DrawString(name, font, textBrush, (float)(left), (float)(top + textPos));
                }
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            currFileIdx++;
            OpenAndIdentifyImage(files[currFileIdx]);

            EnableNextAndPrevButtons();
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            currFileIdx--;
            OpenAndIdentifyImage(files[currFileIdx]);

            EnableNextAndPrevButtons();
        }

        private void EnableNextAndPrevButtons()
        {
            if (currFileIdx < files.Length - 1)
                nextButton.Enabled = true;
            else
                nextButton.Enabled = false;

            if (currFileIdx > 0)
                prevButton.Enabled = true;
            else
                prevButton.Enabled = false;
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            ConfigForm configForm = new ConfigForm();
            configForm.SetApiKey(apiKey);
            configForm.SetURL(url);
            configForm.StartPosition = FormStartPosition.CenterParent;
            DialogResult result = configForm.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                apiKey = configForm.GetApiKey();
                url = configForm.GetURL();

                using (StreamWriter configFile = new StreamWriter("./FaceApi.config"))
                {
                    configFile.WriteLine(apiKey);
                    configFile.WriteLine(url);
                    configFile.Close();
                }
            }
        }
    }
}
