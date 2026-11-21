#!/usr/bin/env python3
"""Build the offline hero catalogue from an MLBBDex data checkout.

Usage:
  python3 tools/build_seed_data.py /path/to/mlbbdex-checkout

The checkout must contain the game JSON files and the hero/item visuals under
public/visuels. ImageMagick's `convert` creates Android-friendly JPEG copies.
"""

from __future__ import annotations

import json
import re
import shutil
import subprocess
import sys
from pathlib import Path

ROLE_NAMES = {
    "Assassin": "Убийца",
    "Fighter": "Боец",
    "Mage": "Маг",
    "Marksman": "Стрелок",
    "Support": "Поддержка",
    "Tank": "Танк",
}
LANE_NAMES = {
    "Exp": "Линия опыта",
    "Gold": "Линия золота",
    "Jungle": "Лес",
    "Mid": "Центр",
    "Roam": "Роум",
}
EMBLEM_NAMES = {
    "All": "Универсальная эмблема",
    "Assassin": "Эмблема убийцы",
    "Fighter": "Эмблема бойца",
    "Mage": "Эмблема мага",
    "Marksman": "Эмблема стрелка",
    "Support": "Эмблема поддержки",
    "Tank": "Эмблема танка",
}
SPELL_NAMES = {
    "Aegis": "Щит",
    "Arrival": "Прибытие",
    "Execute": "Казнь",
    "Flameshot": "Огненный выстрел",
    "Flicker": "Вспышка",
    "Inspire": "Натиск",
    "Petrify": "Окаменение",
    "Purify": "Очищение",
    "Retribution": "Возмездие",
    "Revitalize": "Восстановление",
    "Sprint": "Спринт",
    "Vengeance": "Отмщение",
}
TALENT_NAMES = {
    "Agility": "Ловкость",
    "Bargain Hunter": "Выгодная покупка",
    "Brave Smite": "Храбрый удар",
    "Fatal": "Фатальность",
    "Firmness": "Твёрдость",
    "Concussive Blast": "Сотрясающий взрыв",
    "Focusing Mark": "Метка концентрации",
    "Festival of Blood": "Кровавый пир",
    "Impure Rage": "Нечистая ярость",
    "Inspire": "Воодушевление",
    "Quantum Charge": "Квантовый заряд",
    "Seasoned Hunter": "Опытный охотник",
    "Temporal Reign": "Власть времени",
    "Thrill": "Азарт",
    "Wilderness Blessing": "Благословение дикой природы",
    "Weapon Master": "Мастер оружия",
    "War Cry": "Боевой клич",
    "Killing Spree": "Серия убийств",
    "Lethal Ignition": "Смертоносное воспламенение",
    "Master Assassin": "Мастер убийства",
    "Pull Yourself Together": "Соберись",
    "Rupture": "Разлом",
    "Swift": "Стремительность",
    "Tenacity": "Стойкость",
    "Vitality": "Живучесть",
    "Weakness Finder": "Искатель слабостей",
    "Weapons Master": "Мастер оружия",
}
HERO_NAMES_RU = {
    "aamon": "Эймон", "akai": "Акай", "aldous": "Олдос", "alice": "Алиса",
    "alpha": "Альфа", "alucard": "Алукард", "angela": "Ангела", "argus": "Аргус",
    "arlott": "Арлотт", "atlas": "Атлас", "aulus": "Аулус", "aurora": "Аврора",
    "badang": "Баданг", "balmond": "Бальмонд", "bane": "Бэйн", "barats": "Баратс",
    "baxia": "Баксия", "beatrix": "Беатрис", "belerick": "Белерик", "benedetta": "Бенедетта",
    "brody": "Броди", "bruno": "Бруно", "carmilla": "Кармилла", "cecilion": "Сесилион",
    "chang-e": "Чанъэ", "chip": "Чип", "chou": "Чу", "cici": "Сиси", "claude": "Клод",
    "clint": "Клинт", "cyclops": "Циклоп", "diggie": "Дигги", "dyrroth": "Диррот",
    "edith": "Эдит", "esmeralda": "Эсмеральда", "estes": "Эстес", "eudora": "Эудора",
    "fanny": "Фанни", "faramis": "Фарамис", "floryn": "Флорин", "franco": "Франко",
    "fredrinn": "Фредрин", "freya": "Фрея", "gatotkaca": "Гатоткача", "gloo": "Глу",
    "gord": "Горд", "granger": "Грейнджер", "grock": "Грок", "guinevere": "Гвиневра",
    "gusion": "Гусион", "hanabi": "Ханаби", "hanzo": "Ханзо", "harith": "Харит",
    "harley": "Харли", "hayabusa": "Хаябуса", "helcurt": "Хелкерт", "hilda": "Хильда",
    "hirara": "Хирара", "hylos": "Хилос", "irithel": "Иритэль", "ixia": "Иксия",
    "jawhead": "Джаухед", "johnson": "Джонсон", "joy": "Джой", "julian": "Джулиан",
    "kadita": "Кадита", "kagura": "Кагура", "kaja": "Каджа", "kalea": "Калеа",
    "karina": "Карина", "karrie": "Кэрри", "khaleed": "Халид", "khufra": "Хуфра",
    "kimmy": "Кимми", "lancelot": "Ланселот", "lapu-lapu": "Лапу-Лапу", "layla": "Лейла",
    "leomord": "Леоморд", "lesley": "Лесли", "ling": "Линг", "lolita": "Лолита",
    "lukas": "Лукас", "lunox": "Лунокс", "luo-yi": "Ло И", "lylia": "Лилия",
    "marcel": "Марсель", "martis": "Мартис", "masha": "Маша", "mathilda": "Матильда",
    "melissa": "Мелисса", "minotaur": "Минотавр", "minsitthar": "Минситтар", "miya": "Мия",
    "moskov": "Москов", "nana": "Нана", "natalia": "Наталья", "natan": "Натан",
    "nolan": "Нолан", "novaria": "Новария", "obsidia": "Обсидия", "odette": "Одетта",
    "paquito": "Пакито", "pharsa": "Фарса", "phoveus": "Фовеус", "popol-and-kupa": "Пополь и Купа",
    "rafaela": "Рафаэла", "roger": "Роджер", "ruby": "Руби", "saber": "Сэйбер",
    "selena": "Селена", "silvanna": "Сильванна", "sora": "Сора", "sun": "Сун",
    "suyou": "Сую", "terizla": "Теризла", "thamuz": "Тамуз", "tigreal": "Тигрил",
    "uranus": "Уранус", "vale": "Вэйл", "valentina": "Валентина", "valir": "Валир",
    "vexana": "Вексана", "wanwan": "Ванван", "x-borg": "Икс Борг", "xavier": "Ксавьер",
    "yi-sun-shin": "И Сун-Шин", "yin": "Инь", "yu-zhong": "Юй Чжун", "yve": "Ив",
    "zetian": "Цзэтянь", "zhask": "Жаск", "zhuxin": "Чжусинь", "zilong": "Зилонг",
}


def read_json(path: Path):
    return json.loads(path.read_text(encoding="utf-8"))


def translated(mapping: dict[str, str], key: str) -> str:
    return mapping.get(key, key)


def item_asset_name(name: str) -> str:
    slug = re.sub(r"[^a-z0-9]+", "_", name.lower()).strip("_")
    return f"item_{slug}.jpg"


def item_source_slug(name: str) -> str:
    normalized = name.lower().replace("'s", "-s-").replace("'", "")
    return re.sub(r"[^a-z0-9]+", "-", normalized).strip("-")


def rank_entries(role_data: dict) -> tuple[str, list]:
    for rank in ("all", "mythic", "honor", "glory", "legend", "epic"):
        entries = role_data.get(rank)
        if isinstance(entries, list) and entries:
            return rank, entries
    for rank, entries in role_data.items():
        if isinstance(entries, list) and entries:
            return rank, entries
    return "all", []


def build_record(lane: str, rank: str, raw: dict) -> dict:
    rank_name = "все ранги" if rank == "all" else rank.capitalize()
    return {
        "Title": f"{translated(LANE_NAMES, lane)} · {rank_name}",
        "Lane": lane,
        "Rank": rank,
        "Items": raw.get("items") or [],
        "Emblem": translated(EMBLEM_NAMES, raw.get("emblem") or ""),
        "Talents": [translated(TALENT_NAMES, item) for item in raw.get("talents", [])],
        "Spell": translated(SPELL_NAMES, raw.get("spell") or ""),
        "WinRate": raw.get("winRate"),
        "PickRate": raw.get("pickRate"),
    }


def main(source: Path) -> None:
    source = source.resolve()
    game = source / "src/data/game"
    image_root = source / "public/visuels/heros"
    heroes = read_json(game / "heroes.json")
    statistics = read_json(game / "statistics.json")
    patches = read_json(game / "patches.json")
    sync = read_json(game / "sync.json")
    items = read_json(game / "items.json")
    visuals = read_json(game / "visuals.json")

    destination = Path("Resources/Images")
    destination.mkdir(parents=True, exist_ok=True)
    stale_images = list(destination.glob("hero-*.jpg")) + list(destination.glob("hero_*.jpg"))
    stale_images += list(destination.glob("item_*.jpg"))
    for stale_image in stale_images:
        stale_image.unlink()
    item_images = {}
    item_slugs = {item["name"]: item["slug"] for item in items}
    used_build_items = {
        item_name
        for hero_builds in statistics.get("builds", {}).values()
        for lane_builds in hero_builds.values()
        for entries in lane_builds.values()
        if isinstance(entries, list)
        for entry in entries
        for item_name in entry.get("items", [])
    }
    all_item_names = sorted(set(item_slugs) | used_build_items)
    for name in all_item_names:
        slug = item_slugs.get(name, item_source_slug(name))
        image_name = item_asset_name(name)
        relative_image = visuals.get("items", {}).get(slug, f"/visuels/objets/{slug}.webp")
        source_image = source / "public" / relative_image.lstrip("/")
        if not source_image.is_file():
            raise FileNotFoundError(f"Missing item icon for {name}: {source_image}")
        subprocess.run(
            ["convert", str(source_image), "-quality", "88", str(destination / image_name)],
            check=True,
        )
        item_images[name] = image_name

    records = []
    ids = {hero["slug"] for hero in heroes}

    for hero in heroes:
        slug = hero["slug"]
        source_image = image_root / slug / "portrait.webp"
        if not source_image.is_file():
            raise FileNotFoundError(f"Missing portrait: {source_image}")
        image_name = f"hero_{slug.replace('-', '_')}.jpg"
        subprocess.run(
            ["convert", str(source_image), "-quality", "84", str(destination / image_name)],
            check=True,
        )

        counters = []
        counter_data = statistics.get("counters", {}).get(slug, {})
        for entry in counter_data.get("all", {}).get("weak", [])[:6]:
            target = entry.get("slug")
            if target in ids:
                counters.append({
                    "HeroId": target,
                    "Advantage": round(abs(float(entry.get("advantage", 0))), 1),
                    "Rank": "all",
                })

        builds = []
        hero_builds = statistics.get("builds", {}).get(slug, {})
        for lane in hero.get("lanes", []):
            role_data = hero_builds.get(lane, {})
            rank, entries = rank_entries(role_data)
            builds.extend(build_record(lane, rank, entry) for entry in entries[:3])

        records.append({
            "Id": slug,
            "Name": HERO_NAMES_RU.get(slug, hero["name"]),
            "NameEn": hero["name"],
            "Roles": hero.get("roles", []),
            "Lanes": hero.get("lanes", []),
            "Image": image_name,
            "ImageUrl": None,
            "Counters": counters,
            "Builds": builds,
        })

    patch_list = patches.get("list", [])
    output = {
        "Patch": patch_list[0].get("version", "") if patch_list else "",
        "Updated": sync.get("date", ""),
        "Source": "MLBBDex и Rone Arena",
        "ItemImages": item_images,
        "Heroes": records,
    }
    target = Path("Resources/Raw/heroes.json")
    target.parent.mkdir(parents=True, exist_ok=True)
    target.write_text(json.dumps(output, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {len(records)} heroes to {target}")
    print(f"Wrote {len(item_images)} item icons and portraits to {destination}")


if __name__ == "__main__":
    if len(sys.argv) != 2:
        raise SystemExit("Usage: python3 tools/build_seed_data.py /path/to/mlbbdex-checkout")
    if shutil.which("convert") is None:
        raise SystemExit("ImageMagick `convert` is required to convert WebP portraits to JPEG.")
    main(Path(sys.argv[1]))
