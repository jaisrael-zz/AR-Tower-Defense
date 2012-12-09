#ifndef GUARD_CalibCamera
#define GUARD_CAlibCamera

#include <opencv2/highgui/highgui.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/opencv.hpp>

class CalibCamera
{
public:
	CalibCamera(); //construct an empty `CalibCamera' object
	CalibCamera(float fx, float fy, 
				float cx, float cy); //construct instrinsic camera matrix

    void setInstrinsic(	);
    void setRT( cv::Matx33f H);	
private:
	cv::Matx33f K; //intrinsic matrix
	cv::Matx33f RT; //
}

