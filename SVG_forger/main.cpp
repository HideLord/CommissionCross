#include <iostream>

#include "dictionary.h"
#include "crossword.h"
#include "SVG_element.h"
#include "Smith.h"

#define NOMINMAX
#include <Windows.h>

int main(int argc, char **argv) {
	SetConsoleCP(1251);
	SetConsoleOutputCP(1251);

	int br = 1;
	string crossName;
	if (argc > 1) {
		crossName = argv[br++];
		for (int i = 0; i < crossName.size(); i++) {
			if (crossName[i] == '\\') crossName[i] = '/';
		}
		cout << crossName.size() << crossName << endl;
	}
	else crossName = "xmn6846b.ctb";

	crossword C;
	C.load(crossName);
	C.printASCII();
	Smith S;
	if (argc >= 15) {

		S.setInd = atof(argv[br++]);
		S.squareInd = atof(argv[br++]);
		S.arrowInd = atof(argv[br++]);
		S.setRotation = atof(argv[br++]);
		S.squareRotation = atof(argv[br++]);
		S.setSizeCoefX = atof(argv[br++]) / 100;
		S.setSizeCoefY = atof(argv[br++]) / 100;
		S.squareSizeCoefX = atof(argv[br++]) / 100;
		S.squareSizeCoefY = atof(argv[br++]) / 100;
		S.arrowSizeCoefX = atof(argv[br++]) / 100;
		S.arrowSizeCoefY = atof(argv[br++]) / 100;
		S.setFill = argv[br++];
		S.squareFill = argv[br++];
		S.arrowFill = argv[br++];
	}
	S.setCrossword(C);
	
	S.drawCrossword();
#ifdef DEBUG
	system("aab.svg");
#else // DEBUG
	system("Z:/aprn/aab.svg");
#endif
	S.updateAlternative();
	S.printAlternativesToFile();
	S.printTxtAnswers();
}