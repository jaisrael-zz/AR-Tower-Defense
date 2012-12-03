#include <opencv2/highgui/highgui.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/calib3d/calib3d.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <iostream>

using namespace cv;
using namespace std;

int clicks;
Mat image;
vector<Point2i> im0_corners(4); 

//click 4 points of the plane callback
static void onMouse(int event, int x, int y, int, void*)
{
	if (event != CV_EVENT_LBUTTONDOWN || clicks >= 4)
		return;

	Point center = Point(x,y);
	im0_corners[clicks] = center; 
	circle(image, center, 4, Scalar(0,0,255), -1); 

	clicks++;
	
}
//default do nothing mouse callback
static void onMouse2(int event, int x, int y, int, void*)
{
	return;
}
int main(int argc, char* argv[])
{
	VideoCapture cap(0);
	if(!cap.isOpened()) return -1;

	clicks = 0;
	cap >> image;	
	namedWindow( "Click Points", CV_WINDOW_AUTOSIZE);
	imshow("Click Points", image);	

	setMouseCallback("Click Points", onMouse2, 0);

	int points_clicked = 0;
	int check = 1;

	

	//imshow("Click Points", image);
	cout << "Camera Calibration" << endl;
	Size pattern(7,7);


	//calibrate camera
	vector<vector<Point3f> > object_points;
	
	vector<Point3f> obj;
	for (int x = 0; x < pattern.height; x++)
	{
		for (int y = 0; y < pattern.width; y++)
		{
			obj.push_back(Point3f(x,y,0));
		}
	}
	int num_images = 0;
	vector<vector<Point2f> > image_points;
	vector<Point2f> corners;
	while (check)
	{
		cap >> image;
		char key = waitKey(30);

		switch(key) {
		case 27:
			cout << "ESC was pressed" << endl;
			if (num_images > 0) check = 0;
			break;
		case 'r':
			if (findChessboardCorners(image, pattern, corners))
			{
				drawChessboardCorners(image, pattern, corners, true);
				image_points.push_back(corners);
				object_points.push_back(obj);
				num_images++;
			}
			else
				drawChessboardCorners(image, pattern, corners, false);
			//check = 0;
			imshow("Click Points", image);
		    waitKey(1000);	
			
		default:
			imshow("Click Points", image);
		}
	}
    
    Mat cameraMatrix = Mat::eye(3, 3, CV_64F);
	Mat distCoeffs = Mat::zeros(8, 1, CV_64F);
	vector<Mat> rvecs, tvecs;
	cout << cameraMatrix << endl;
	cout << distCoeffs << endl;
	double rpe = calibrateCamera(object_points, image_points, image.size(), cameraMatrix, distCoeffs,
								 rvecs, tvecs);

	cout << "Camera Matrix:" << endl << cameraMatrix << endl;
	cout << "distCoeffs:" << endl << distCoeffs << endl;
	cout << "reprojection error: " << rpe << endl;


	check = 1;
	cout << "Click 4 points of your plane" << endl;
    //click 4 points
    while (check)
	{
		cap >> image;
		char key = waitKey(30);

		switch(key) {
		case 27:
			cout << "ESC was pressed" << endl;
			check = 0;
            break;
		case 'r':
			if (points_clicked)
				break;	
			clicks = 0;	
			setMouseCallback("Click Points", onMouse, 0);
			while (clicks < 4)
			{
				imshow("Click Points", image);
				waitKey(1);
			}
			setMouseCallback("Click Points", onMouse2, 0);
			points_clicked = 1;

		default:
			imshow("Click Points", image);
		}
	}

	cout << "Clicked Plane" << endl
		 << "(x,y)" << endl 
		 << "(" << im0_corners[0].x << "," << im0_corners[0].y << ")" << endl
		 << "(" << im0_corners[1].x << "," << im0_corners[1].y << ")" << endl
		 << "(" << im0_corners[2].x << "," << im0_corners[2].y << ")" << endl
		 << "(" << im0_corners[3].x << "," << im0_corners[3].y << ")" << endl;

}


