#include <opencv2/highgui/highgui.hpp>
#include <opencv2/core/core.hpp>
#include <iostream>

using namespace cv;
using namespace std;

int clicks;
Mat image;
std::vector<Point2i> im0_corners(4); 

static void onMouse(int event, int x, int y, int, void*)
{
	if (event != CV_EVENT_LBUTTONDOWN || clicks >= 4)
		return;

	Point center = Point(x,y);
	im0_corners[clicks] = center; 
	circle(image, center, 4, Scalar(0,0,255), -1); 

	clicks++;
	
}
int main(int argc, char* argv[])
{
	clicks = 0;

	if (argc != 2)
	{
		cout << "Usage: click_points <image file>" << endl;
		return -1;
	}
	image = imread( argv[1], -1);
	if (!image.data )
	{
		cout << "Could not read image data" << endl;
		return -1;
	}

	
	namedWindow( "Click Points", CV_WINDOW_AUTOSIZE);
	imshow("Click Points", image);	

	setMouseCallback("Click Points", onMouse, 0);
	while (clicks < 4)
	{
		imshow("Click Points", image);
		if (waitKey(15) == 27) break;
	}

	imshow("Click Points", image);
	waitKey(0);

	cout << "(x,y)" << endl 
		 << "(" << im0_corners[0].x << "," << im0_corners[0].y << ")" << endl
		 << "(" << im0_corners[1].x << "," << im0_corners[1].y << ")" << endl
		 << "(" << im0_corners[2].x << "," << im0_corners[2].y << ")" << endl
		 << "(" << im0_corners[3].x << "," << im0_corners[3].y << ")" << endl;

}


