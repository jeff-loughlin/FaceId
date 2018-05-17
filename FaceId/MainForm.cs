﻿#define USE_OPENCV

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
        private OpenCvSharp.Rect [] DetectFacesInImage(Mat img)
        {
            if (classifier == null)
            {
                classifier = new CascadeClassifier(@"E:\opencv\sources\data\haarcascades\haarcascade_frontalface_alt.xml");
//                var nestedCascade = new CascadeClassifier(@"..\..\Data\haarcascade_eye_tree_eyeglasses.xml");
            }

            OpenCvSharp.Rect [] faces = classifier.DetectMultiScale(
                 image: img,
                 scaleFactor: 1.1,
                 minNeighbors: 2,
                 flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                 minSize: new OpenCvSharp.Size(30, 30));
            if (faces.Length != 1)
            {
                Console.WriteLine("Too many or not enough faces detected.");
            }

            return faces;
        }
#endif

        private async void Form1_Load(object sender, EventArgs e)
        {
#if USE_OPENCV
            int numTrainingFaces = 6;
            Mat[] images = new Mat [numTrainingFaces];
            int[] labels = new int [numTrainingFaces];

            for (int n = 0; n < numTrainingFaces; n++)
                images[n] = new Mat();

          
            Mat image = new Mat("e:/pics/DS2_0541.jpg", ImreadModes.GrayScale);
            OpenCvSharp.Rect[] faces = DetectFacesInImage(image);
            Mat crop = new Mat(image, faces[0]);
            Cv2.Resize(crop, images[0], new OpenCvSharp.Size(400,400));
            labels[0] = 0;

            image = new Mat("e:/pics/DS2_0903-crop.jpg", ImreadModes.GrayScale);
            faces = DetectFacesInImage(image);
            crop = new Mat(image, faces[0]);
            Cv2.Resize(crop, images[1], new OpenCvSharp.Size(400, 400));
            labels[1] = 0;

            image = new Mat("e:/pics/DS2_1013.jpg", ImreadModes.GrayScale);
            faces = DetectFacesInImage(image);
            crop = new Mat(image, faces[0]);
            Cv2.Resize(crop, images[2], new OpenCvSharp.Size(400, 400));
            labels[2] = 0;

            image = new Mat("e:/pics/DS2_1014.jpg", ImreadModes.GrayScale);
            faces = DetectFacesInImage(image);
            crop = new Mat(image, faces[0]);
            Cv2.Resize(crop, images[3], new OpenCvSharp.Size(400, 400));
            labels[3] = 0;

            image = new Mat("e:/pics/3037 - portrait crop.jpg", ImreadModes.GrayScale);
            faces = DetectFacesInImage(image);
            crop = new Mat(image, faces[0]);
            Cv2.Resize(crop, images[4], new OpenCvSharp.Size(400, 400));
            labels[4] = 1;

            image = new Mat("e:/pics/DS2_0433-portrait.jpg", ImreadModes.GrayScale);
            faces = DetectFacesInImage(image);
            crop = new Mat(image, faces[0]);
            Cv2.Resize(crop, images[5], new OpenCvSharp.Size(400, 400));
            labels[5] = 1;

            for (int n = 0; n < numTrainingFaces; n++)
            {
                Cv2.ImShow("zzz", images[n]);
                Cv2.WaitKey(0);
            }

            OpenCvSharp.Face.FaceRecognizer faceRecognizer = OpenCvSharp.Face.FisherFaceRecognizer.Create();
            faceRecognizer.Train(images, labels);
            faceServiceClient = null;

            Mat img = new Mat("e:/pics/DS2_3832.jpg", ImreadModes.GrayScale);
            faces = DetectFacesInImage(img);
            Mat testImage1 = new Mat();
            crop = new Mat(img, faces[0]);
            Cv2.Resize(crop, testImage1, new OpenCvSharp.Size(400, 400));
            Cv2.ImShow("zzz", testImage1);
            Cv2.WaitKey(0);
            int result = faceRecognizer.Predict(testImage1);

            img = new Mat("e:/pics/DS2_4522.jpg", ImreadModes.GrayScale);
            faces = DetectFacesInImage(img);
            Mat testImage2 = new Mat();
            crop = new Mat(img, faces[0]);
            Cv2.Resize(crop, testImage2, new OpenCvSharp.Size(400, 400));
            Cv2.ImShow("zzz", testImage2);
            Cv2.WaitKey(0);
            int result2 = faceRecognizer.Predict(testImage2);

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
            catch (FileNotFoundException ex)
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
            personGroups = await GetPersonGroups();
#endif
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
            var personGroups = await faceServiceClient.ListPersonGroupsAsync();
            return personGroups.ToArray();
        }

        private async Task<Person []> GetPersonsInPersonGroup(string personGroupId)
        {
            var persons = await faceServiceClient.ListPersonsInPersonGroupAsync(personGroupId);
            return persons.ToArray();
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
                    await faceServiceClient.CreatePersonGroupAsync(personGroupSelector.Text.ToLower(), personGroupSelector.Text);
                    personGroups = await GetPersonGroups();
                    personGroupSelector.SelectedIndex = 0;
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
                    await faceServiceClient.CreatePersonInPersonGroupAsync(selectedPersonGroup.PersonGroupId, personSelector.Text);
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
            await faceServiceClient.TrainPersonGroupAsync(selectedPersonGroup.PersonGroupId);
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
                Color penColor = Color.MediumSpringGreen;
                Pen pen = new Pen(penColor, 2f);

                e.Graphics.DrawRectangle(pen, scaledRect);


                string faceAttributes = String.Format("Age {0}|{1}", Math.Round(face.FaceAttributes.Age, MidpointRounding.AwayFromZero), face.FaceAttributes.Gender);
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


                string name = (string)faceNames[n];
                int textPos = -15;
                Font font = new Font(FontFamily.GenericSansSerif, 10);
                SolidBrush brush = new SolidBrush(penColor);
                if (name == "Unknown" && faceAttributes != null)
                {
                    string [] parts = faceAttributes.Split('|');
                    for (int m = 0; m < parts.Length; m++)
                    {
                        string s;
                        if (m == 1)
                            s = String.Format("{0} {1}", name, parts[m]);
                        else
                            s = parts[m];
                        e.Graphics.DrawString(s, font, brush, (float)(left), (float)(top + textPos));
                        textPos -= 15;
                    }
                }
                else
                    e.Graphics.DrawString(name, font, brush, (float)(left), (float)(top + textPos));
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
