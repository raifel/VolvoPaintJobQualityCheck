using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volvo.PaintJobCheck.ImageProcessing;
using System.Drawing;
using motion;

namespace ImageProcessingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MotionDetector3 motionDetector = new MotionDetector3();
            motionDetector.MotionLevelCalculation = true;
            
            var bitmap1 = new Bitmap(@".\images\1.jpg");
            var bitmap2 = new Bitmap(@".\images\2.jpg");
            motionDetector.ProcessFrame(ref bitmap1);
            motionDetector.ProcessFrame(ref bitmap2);

            if (motionDetector.MotionLevel > 0)
            {
                MotionDetector1 motionDetector1 = new MotionDetector1();
                motionDetector1.ProcessFrame(ref bitmap1);
                motionDetector1.ProcessFrame(ref bitmap2);
                bitmap2.Save(@".\images\3.jpg");

            }


        }
    }
}
