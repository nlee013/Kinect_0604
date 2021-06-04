using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace _0604_kinect
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor _sensor = null;
        private ColorFrameReader _colorFrameReader = null;
        private WriteableBitmap _bitmap = null;

        public ImageSource ImageSource
        {
            get { return _bitmap; }
        }

        public MainWindow()
        {
            _sensor = KinectSensor.GetDefault();
            _colorFrameReader = _sensor.ColorFrameSource.OpenReader();//colourFrameReader 객체 불러오기

            FrameDescription description = _sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            _bitmap = new WriteableBitmap(description.Width, description.Height, 96, 96, PixelFormats.Bgr32, null);

            _sensor.Open();

            _colorFrameReader.FrameArrived += _colorFrameReader_FrameArrived;

            this.DataContext = this;
            InitializeComponent();

        }


        private void _colorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if(colorFrame != null)
                {
                    FrameDescription des = colorFrame.FrameDescription; //매 프레임마다 현재 이미지 받아옴
                    using(KinectBuffer buffer = colorFrame.LockRawImageBuffer())
                    {
                        _bitmap.Lock();

                        colorFrame.CopyConvertedFrameDataToIntPtr(_bitmap.BackBuffer, (uint)(des.Width * des.Height * 4), ColorImageFormat.Bgra);
                        _bitmap.AddDirtyRect(new Int32Rect(0, 0, 1920, 1080));

                        _bitmap.Unlock();
                        
                    }
                }
            }
            //frame이 생성될 떄마다 함수 할당 /냥이
        }
    }
}
