from argparse import ArgumentParser
import shutil
import os
from pathlib import Path

if __name__ == "__main__":

    parser = ArgumentParser()
    parser.add_argument("--export")

    _, dirs, _ = next(os.walk("examples"))

    os.makedirs("exported", exist_ok=True)

    for d in dirs:
        path_out_no_suffix = Path("exported", d)
        path_in = Path("examples", d)

        shutil.make_archive(
            base_name=path_out_no_suffix,
            format="zip",
            root_dir=path_in,
            base_dir=".",
        )

        path_zip_suffix = f"{path_out_no_suffix}.zip"
        path_no_suffix, _ = os.path.splitext(path_out_no_suffix)
        path_fmu_suffix = f"{path_no_suffix}.fmu"
        shutil.move(path_zip_suffix, path_fmu_suffix)
