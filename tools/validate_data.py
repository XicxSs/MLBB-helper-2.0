#!/usr/bin/env python3
"""Check the bundled offline catalogue and its image assets before packaging."""

import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CATALOG_PATH = ROOT / "Resources/Raw/heroes.json"
IMAGE_DIR = ROOT / "Resources/Images"
VALID_LANES = {"Exp", "Gold", "Jungle", "Mid", "Roam"}


def main() -> None:
    data = json.loads(CATALOG_PATH.read_text(encoding="utf-8"))
    heroes = data.get("Heroes", [])
    if len(heroes) < 100:
        raise SystemExit(f"Expected the full hero roster; found only {len(heroes)} entries.")
    if not data.get("Patch"):
        raise SystemExit("The bundled catalogue has no patch version.")

    ids = [hero.get("Id", "") for hero in heroes]
    if len(ids) != len(set(ids)) or any(not hero_id for hero_id in ids):
        raise SystemExit("Hero IDs must be present and unique.")
    known_ids = set(ids)
    item_images = data.get("ItemImages", {})
    if not item_images:
        raise SystemExit("The offline catalogue has no item-icon map.")
    for item_name, image in item_images.items():
        if not (IMAGE_DIR / image).is_file():
            raise SystemExit(f"Missing item icon for {item_name}: {image}")

    for hero in heroes:
        slug = hero["Id"]
        if not hero.get("Name") or not hero.get("NameEn"):
            raise SystemExit(f"Missing Russian or English name for {slug}.")
        if not re.search(r"[А-Яа-яЁё]", hero["Name"]):
            raise SystemExit(f"The Russian display name for {slug} is not localized.")
        unknown_lanes = set(hero.get("Lanes", [])) - VALID_LANES
        if unknown_lanes:
            raise SystemExit(f"Unknown lane code(s) for {slug}: {', '.join(sorted(unknown_lanes))}.")
        image = hero.get("Image", "")
        if not image or not (IMAGE_DIR / image).is_file():
            raise SystemExit(f"Missing bundled portrait for {slug}: {image}")
        for counter in hero.get("Counters", []):
            if counter.get("HeroId") not in known_ids:
                raise SystemExit(f"Unknown counter hero {counter.get('HeroId')} for {slug}.")
        for build in hero.get("Builds", []):
            if not build.get("Items"):
                raise SystemExit(f"Build without items for {slug}.")
            for item_name in build["Items"]:
                if item_name not in item_images:
                    raise SystemExit(f"No offline item icon for {item_name} in {slug}'s build.")

    print(f"Validated {len(heroes)} heroes, {len(ids)} unique IDs, {len(item_images)} item icons and {len(heroes)} portraits (patch {data['Patch']}).")


if __name__ == "__main__":
    main()
