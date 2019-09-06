
#pragma once
#include <iostream>
#include <fstream>
#include <algorithm>
#include <cmath>
#include <vector>
#include <memory>
#include <string>
#include <Windows.h>
#include <wchar.h>
#include <map>

using namespace std;

namespace SVG {
	class SVG_element {
	public:
		void print(ostream & out) const {
			out << "<" << name << " ";
			for (auto i : attributes) {
				out << i.first << "=\"" << i.second << "\" ";
			}
			out << ">" << flush;
			if (!content.empty())out << content << flush;
			for (int i = 0; i < children.size(); i++) {
				children[i]->print(out);
			}
			out << "</" << name << ">";
		}

		void add(shared_ptr<SVG_element> E) {
			children.push_back(std::move(E));
		}
		void add_copy(SVG_element E) {
			children.push_back(std::make_shared<SVG_element>());
			*children.back() = E;
		}

		static wstring to_wide(string s) {
			const wstring cyrillic = L"АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ ";
			using uc = unsigned char;
			const int cyrillicA = 224;
			wstring res;
			for (int i = 0; i < s.size(); i++) {
				uc c = s[i];
				if (c >= cyrillicA - 32 && c < cyrillicA)res.push_back(cyrillic[c + 32 - cyrillicA]);
				else if (c >= cyrillicA && c < cyrillicA + 32)res.push_back(cyrillic[c - cyrillicA]);
				else res.push_back(c);
			}
			return res;
		}
		static string to_ascii(wstring s) {
			const wstring cyrillic  = L"АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
			const wstring cyrillicS = L"абвгдежзийклмнопрстуфхцчшщъьььюя";

			using uc = unsigned char;

			string res;
			for (int i = 0; i < s.size(); i++) {
				int upper = -1,lower = -1;
				for (int j = 0; j < cyrillic.size(); j++)
					if (cyrillic[j] == s[i]){
						upper = j;
						break;
					}
				for (int j = 0; j < cyrillicS.size(); j++)
					if (cyrillicS[j] == s[i]) {
						lower = j;
						break;
					}

				const int cyrillica = 224;
				const int cyrillicA = 224-32;

				if (upper != -1) {
					res.push_back(uc(cyrillicA + upper));
					continue;
				}
				if (lower != -1) {
					res.push_back(uc(cyrillica + lower));
					continue;
				}

				res.push_back(s[i]);
			}

			return res;
		}


		friend ostream& operator<<(ostream & out, SVG_element & s) {
			out << "<" << s.name << " ";
			for (auto i : s.attributes) {
				out << i.first << "=\"" << i.second << "\" ";
			}
			out << ">" << flush;
			if (!s.content.empty())out << s.content << flush;
			for (int i = 0; i < s.children.size(); i++) {
				out << (*s.children[i]);
			}
			out << "</" << s.name << ">";
			return out;

		}

		void addAttribute(string att, string value) {
			attributes[att] = value;
		}
		void setName(string name) {
			this->name = name;
		}
		void setAttributes(map<string, string> attributes) {
			this->attributes = attributes;
		}
		void setContent(string content) {
			this->content = content;
		}

		SVG_element() {
		}

		SVG_element(string name) {
			setName(name);
		}
		SVG_element(string name, string content) {
			setName(name);
			setContent(content);
		}
		SVG_element(string name, map<string, string> attributes) {
			setName(name);
			setAttributes(attributes);
		}
		SVG_element(string name, map<string, string> attributes, string content) {
			setName(name);
			setContent(content);
			setAttributes(attributes);
		}
		string & operator[](string key) {
			return attributes[key];
		}
	protected:
		string name, content;
		map<string, string> attributes;
		vector<shared_ptr<SVG_element>> children;
	};
    inline string GetCMYK(){
        string color;
        double c,m,y,k;
        cin >> c >> m >> y >> k;
        c/=100;
        m/=100;
        y/=100;
        k/=100;
        int r = 255 * (1.0-c) * (1.0-k);
        int g = 255 * (1.0-m) * (1.0-k);
        int b = 255 * (1.0-y) * (1.0-k);
        color = "rgb("+to_string(r)+","+to_string(g)+","+to_string(b)+")";
        return color;
    }
	inline SVG_element GetRoot(double width, double height) {
		return SVG_element("svg", {
			{"width",to_string(width)+"mm"},
			{"height",to_string(height)+"mm"},
			{"viewBox","0 0 " + to_string(width) + " " + to_string(height)},
			{"xmlns", "http://www.w3.org/2000/svg"},
			{"version","1.1"}
			});
	}
	inline SVG_element GetText(double x = 0, double y = 0, string content = "empty") {
		return SVG_element("text", {
			{"x",to_string(x)},
			{"y",to_string(y)},
			{"text-anchor","middle"}
			}, content);
	}
	inline SVG_element GetBox(double x = 0, double y = 0, double szW = 10, double szH = 10, double stroke_width = 2) {
		return SVG_element("rect", {
			{"x",to_string(x)},
			{"y",to_string(y)},
			{"width",to_string(szW)},
			{"height",to_string(szH)},
			{"stroke-width",to_string(stroke_width)},
			{"fill","white"},
			{"stroke","black"}
			});
	}
}

