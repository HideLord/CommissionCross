#include "Smith.h"

#include <codecvt>
#include <sstream>

void Smith::updateAlternative()
{
	string file = SVG::SVG_element::to_ascii(readFinishedSVG("Z:/aprn/aab.svg"));

	size_t nextText = 0, i = 0;
	nextText = file.find("<text", i);
	int mainbreaker = 0;
	while (++mainbreaker < 1000000 && nextText != string::npos) {
		size_t closing = min(file.find("</text", nextText), file.find("/>", nextText + 5));
		if (closing == string::npos) {
			break;
		}
		size_t wordStart = file.find("id=\"", nextText);
		size_t wordEnd = file.find("\"", wordStart + 4);

		string word, clue;
		if (wordStart != string::npos && wordEnd != string::npos) {
			for (i = wordStart + 4; i < wordEnd; i++) {
				word.push_back(file[i]);
			}
		}
		if (word.empty()) {
			nextText = file.find("<text", closing);
			continue;
		}

		size_t lineStart = nextText;
		size_t lineEnd = nextText;

		int lineBr = 0;

		bool notValid = false;

		for (int breaker = 0; breaker < 1000; breaker++) {
			lineStart = file.find("<tspan", lineEnd);
			if (lineStart == string::npos || lineStart >= closing)break;
			lineStart = file.find(">", lineStart);
			if (lineStart == string::npos || lineStart >= closing)break;
			lineEnd = file.find("</tspan", lineStart);
			if (lineEnd == string::npos || lineEnd >= closing)break;
			
			
			string tempClueLine;
			for (i = lineStart + 1; i < lineEnd; i++)tempClueLine.push_back(file[i]);
			replace(tempClueLine, "&quot;", "\"");

			//The clue has a line which is over 15 characters long.
			//so probably something went wrong
			if (tempClueLine.size() >= 16 && word != "dict") {
				notValid = true;
				break;
			}

			clue += tempClueLine;
			clue.push_back('\n');
			lineBr++;
		}
		while(!clue.empty() && clue.back() == '\n')clue.pop_back();

		if (clue.empty() || lineBr > 10 || notValid) {
			nextText = file.find("<text", closing);
			continue;
		}

		replace(clue, "&quot;", "\"");

		if (alternatives.count(word)) {
			if (!alternatives[word].count(clue)) {
				alternatives[word][clue] = lineBr;
			}
		}
		else {
			alternatives[word] = map<string, int> { {clue, lineBr} };
		};

		nextText = file.find("<text", closing);
	} 
}

void Smith::printAlternativesToFile()
{
	ofstream fout("altDictTemp.txt");

	for (auto & i : alternatives) {
		for (auto & j : i.second) {
			fout << i.first << endl;
			fout << j.first << endl;
			fout << "!!!" << endl;
		}
	}

	fout.close();

	remove("Z:\\Cross\\altDictTemp.txt");
	rename("Z:\\Cross\\altDict.txt", "Z:\\Cross\\altDictTemp.txt");
	rename("altDictTemp.txt", "Z:\\Cross\\altDict.txt");
}

void Smith::printTxtAnswers()
{
	ofstream fout(string("Z:\\Cross\\" + cross.name + ".txt").c_str());
	vector<string> NameClues;
	for (size_t i = 0; i < cross.areas.size(); i++) {
		string temp = dict.explanationDict[cross.areas[i].to_string()], NameClue;
		bool secondName = false;
		for (size_t j = 0; j < temp.size(); j++) {
			if (temp[j] == '/') {
				j++;
				while (temp[j] != '/') {
					if(!secondName)NameClue.push_back(temp[j++]);
					else ++j;
				}
				secondName = true;
			}
		}
		if (NameClue.back() == ' ')NameClue.pop_back();
		NameClues.push_back(NameClue);
	}
	fout << "ÂÎÄÎÐÀÂÍÎ: ";
	
	for (int i = 0; i < words.size(); i++) {
		if (!words[i].hor)continue;
		string areaWord = cross.areas[words[i].areaInd].to_string();
		string unedited = (dict.dirtyDict.count(areaWord) ? dict.dirtyDict[areaWord] : areaWord);
		for (int j = 0; j < unedited.size(); j++) {
			if (uc(unedited[j]) > crossword::cyrillicA - 33 && uc(unedited[j]) < crossword::cyrillicA + 32) {
				if (uc(unedited[j]) >= crossword::cyrillicA && uc(unedited[j]) < crossword::cyrillicA + 32)unedited[j] -= 32;
				break;
			}
		}
		fout << unedited;
		if (!NameClues[words[i].areaInd].empty())fout << " (" + NameClues[words[i].areaInd] + ")";
		fout << ". ";
	}

	auto wordsTemp = words;
	sort(wordsTemp.begin(), wordsTemp.end(), [](duma& A, duma& B) -> bool{
		if (A.j == B.j)return A.i < B.i;
		return A.j < B.j;
	});

	fout << endl << "ÎÒÂÅÑÍÎ: ";
	
	for (int i = 0; i < wordsTemp.size(); i++) {
		if (wordsTemp[i].hor)continue;
		string areaWord = cross.areas[wordsTemp[i].areaInd].to_string();
		string unedited = (dict.dirtyDict.count(areaWord) ? dict.dirtyDict[areaWord] : areaWord);
		for (int j = 0; j < unedited.size(); j++) {
			if (uc(unedited[j]) > crossword::cyrillicA - 33 && uc(unedited[j]) < crossword::cyrillicA + 32) {
				if (uc(unedited[j]) >= crossword::cyrillicA && uc(unedited[j]) < crossword::cyrillicA + 32)unedited[j] -= 32;
				break;
			}
		}
		fout << unedited;
		if (!NameClues[wordsTemp[i].areaInd].empty())fout << " (" + NameClues[wordsTemp[i].areaInd] + ")";
		fout << ". ";
	}
	fout.close();

}

wstring Smith::readFinishedSVG(string filename)
{
	std::wifstream wif(filename);
	wif.imbue(std::locale(std::locale::empty(), new std::codecvt_utf8<wchar_t>));
	std::wstringstream wss;
	wss << wif.rdbuf();
	return wss.str();
}
