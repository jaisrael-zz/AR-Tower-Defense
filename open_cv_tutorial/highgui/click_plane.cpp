#include <opencv2/highgui/highgui.hpp>
#include <opencv2/core/core.hpp>
#include <iostream>

using namespace cv;
using namespace std;

int clicks;
Mat image;

static void onMouse(int event, int x, int y, int, void*)
{
	if (event != CV_EVENT_LBUTTONDOWN)
		return;

	Point center = Point(x,y);

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

}


