import os
import argparse
import sys
import html

import pandas as pd
from translate import Translator

parser = argparse.ArgumentParser(description="translate csvs in text")
parser.add_argument("--language", type=str, required=True)
parser.add_argument("--column-name", type=str, required=True)

args = parser.parse_args()

print(f"Translating from english to {args.language}. Writing to new "\
      f"column '{args.column_name}'")

THIS_DIR = os.path.dirname(os.path.abspath(__file__))
ROOT_BOATVS_DIR = os.path.abspath(os.path.join(THIS_DIR, ".."))
TEXT_DIR = os.path.join(ROOT_BOATVS_DIR, "text")

files_to_translate = [os.path.join(TEXT_DIR, "foods.csv"),
                      os.path.join(TEXT_DIR, "tutorial.csv"),
                      os.path.join(TEXT_DIR, "ui.csv")]
def main():
    for file_path in files_to_translate:
        print(f"Translating {file_path}...")
        df = pd.read_csv(file_path)
        df[args.column_name] = df.apply(lambda row: translate(row), axis = 1)
        df.to_csv(file_path, index=False)

def translate(row):
    translator = Translator(from_lang="english", to_lang=args.language)
    return html.unescape(translator.translate(row["en"]))

if __name__ == "__main__":
    main()
