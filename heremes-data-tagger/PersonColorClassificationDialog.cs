﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HermesDataTagger
{
    public partial class PersonColorClassificationDialog : Form
    {
        public TaggedPerson Person { get; private set; }
        private bool _isSettingShirtColor = true;
        private bool _isSettingShortsColor = false;
        private bool _isSettingShoesColor = false;

        public PersonColorClassificationDialog(TaggedPerson person)
        {
            InitializeComponent();
            Person = person;
            CropPersonPhoto();
            BindDataToControls();
            BindEvents();
            UpdateInstructionsLabel();
            // TODO: Work out why the window size won't reflect the designer
            Height += 158;
        }

        void CropPersonPhoto()
        {
            Image srcImg = MainWindow.Singleton.MainPictureBox.Image;
            // Crop image to bib
            Bitmap bmp = new Bitmap(srcImg);
            // Bounding area
            int left = Person.LeftmostPixelX - 200;
            int right = Person.RightmostPixelX + 200;
            // Ensure we don't overcompensate
            left = left < 0 ? 0 : left;
            right = right > srcImg.Width ? srcImg.Width : right;
            int width = right - left;
            int height = srcImg.Height;
            Rectangle crop = new Rectangle(left, 0, width, height);
            // Clone cropped image
            Bitmap croppedImage = bmp.Clone(crop, bmp.PixelFormat);
            bmp.Dispose();
            // Present
            imgPersonCrop.Image = croppedImage;
        }

        void BindDataToControls()
        {
            pnlShirtColor.DataBindings.Add("BackColor", Person, "ShirtColor", false, DataSourceUpdateMode.OnPropertyChanged, SystemColors.Control);
            pnlShortsColor.DataBindings.Add("BackColor", Person, "ShortsColor", false, DataSourceUpdateMode.OnPropertyChanged, SystemColors.Control);
            pnlShoesColor.DataBindings.Add("BackColor", Person, "ShoesColor", false, DataSourceUpdateMode.OnPropertyChanged, SystemColors.Control);
        }

        void BindEvents()
        {
            imgPersonCrop.MouseClick += OnClickImage;
            // Manually bind radio change events
            rdoSettingShirtColor.CheckedChanged += (sender, e) =>
            {
                _isSettingShirtColor = !_isSettingShirtColor;
                UpdateInstructionsLabel();
            };
            rdoSettingShoesColor.CheckedChanged += (sender, e) =>
            {
                _isSettingShoesColor = !_isSettingShoesColor;
                UpdateInstructionsLabel();
            };
            rdoSettingShortsColor.CheckedChanged += (sender, e) =>
            {
                _isSettingShortsColor = !_isSettingShortsColor;
                UpdateInstructionsLabel();
            };
            // Clear buttons
            btnClearShirtColor.Click += (sender, e) => Person.ShirtColor = Color.Empty;
            btnClearShortsColor.Click += (sender, e) => Person.ShortsColor = Color.Empty;
            btnClearShoesColor.Click += (sender, e) => Person.ShoesColor = Color.Empty;
        }

        void UpdateInstructionsLabel()
        {
            if (_isSettingShirtColor)
            {
                lblInstructions.Text = "Please click on the shirt of the runner to set the color, or skip if not visible";
            }
            if (_isSettingShortsColor)
            {
                lblInstructions.Text = "Please click on the shorts of the runner to set the color, or skip if not visible";
            }
            if (_isSettingShoesColor)
            {
                lblInstructions.Text = "Please click on the shoe of the runner to set the color, or skip if not visible";
            }
        }

        private void OnClickImage(object sender, MouseEventArgs e)
        {
            Point pt = e.Location;
            // Ignore this point if outside bounds of pbx
            if (!imgPersonCrop.IsPointInImage(pt))
            {
                return;
            }
            Bitmap bmp = (Bitmap)(imgPersonCrop.Image);
            Point pixelPt = pt.ToPixelPoint(imgPersonCrop);
            Color pixelColor = bmp.GetPixel(pixelPt.X, pixelPt.Y);
            if (_isSettingShirtColor)
            {
                Person.ShirtColor = pixelColor;
                rdoSettingShortsColor.Checked = true;
            }
            else if (_isSettingShortsColor)
            {
                Person.ShortsColor = pixelColor;
                rdoSettingShoesColor.Checked = true;

            }
            else
            {
                Person.ShoesColor = pixelColor;
            }
        }
    }
}
