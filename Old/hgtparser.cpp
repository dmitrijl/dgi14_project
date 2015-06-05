#include <iostream>
#include <fstream>

const int SRTM_SIZE = 1201;
short height[SRTM_SIZE][SRTM_SIZE] = {0};

int main(int argc, const char * argv[])
{
	std::ifstream file("N59E018.hgt", std::ios::in|std::ios::binary);
	if(!file)
	{
		std::cout << "Error opening file!" << std::endl;
		return -1;
	}

	unsigned char buffer[2];
	for (int i = 0; i < SRTM_SIZE; ++i)
	{
		for (int j = 0; j < SRTM_SIZE; ++j) 
		{
			if(!file.read( reinterpret_cast<char*>(buffer), sizeof(buffer) ))
			{
				std::cout << "Error reading file!" << std::endl;
				return -1;
			}
			height[i][j] = (buffer[0] << 8) | buffer[1];
		}
	}

	//Read single value from file at row,col
	/*
	const int row = 500;
	const int col = 1000;
	size_t offset = sizeof(buffer) * ((row * SRTM_SIZE) + col);
	file.seekg(offset, std::ios::beg);
	file.read( reinterpret_cast<char*>(buffer), sizeof(buffer) );
	short single_value = (buffer[0] << 8) | buffer[1];
	std::cerr << "values at " << row << "," << col << ":" << std::endl;
	std::cerr << "  height array: " << height[row][col] << ", file: " << single_value << std::endl;
	*/
	
	short min = 32767;
	short max = -32768;

	for (int i = 0;i < SRTM_SIZE;++i) {
		for (int j = 0;j < SRTM_SIZE;++j) {
			fprintf(stdout, "%d ", height[i][j]);
			if (height[i][j] > max) {
				max = height[i][j];
			}
			if (height[i][j] < min) {
				min = height[i][j];
			}
		}
	}
	
	fprintf(stderr, "Min and max values are %d and %d.\n", min, max);

	return 0;
}

