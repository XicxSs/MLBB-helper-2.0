using System.Text;

namespace MlbbHelper.Services;

public static class GameLocalization
{
    static readonly (string Latin, string Cyrillic)[] LetterPairs =
    {
        ("shch", "щ"), ("zh", "ж"), ("kh", "х"), ("ch", "ч"), ("sh", "ш"),
        ("ts", "ц"), ("yu", "ю"), ("ya", "я"), ("yo", "ё"), ("ph", "ф"),
        ("th", "т"), ("ng", "нг"), ("qu", "кв")
    };

    static readonly Dictionary<char, string> Letters = new()
    {
        ['a'] = "а", ['b'] = "б", ['c'] = "к", ['d'] = "д", ['e'] = "э",
        ['f'] = "ф", ['g'] = "г", ['h'] = "х", ['i'] = "и", ['j'] = "дж",
        ['k'] = "к", ['l'] = "л", ['m'] = "м", ['n'] = "н", ['o'] = "о",
        ['p'] = "п", ['q'] = "к", ['r'] = "р", ['s'] = "с", ['t'] = "т",
        ['u'] = "у", ['v'] = "в", ['w'] = "в", ['x'] = "кс", ['y'] = "й", ['z'] = "з"
    };

    static readonly Dictionary<string, string> Roles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Assassin"] = "Убийца",
        ["Fighter"] = "Боец",
        ["Mage"] = "Маг",
        ["Marksman"] = "Стрелок",
        ["Support"] = "Поддержка",
        ["Tank"] = "Танк",
    };

    static readonly Dictionary<string, string> Lanes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Exp"] = "Линия опыта",
        ["Gold"] = "Линия золота",
        ["Jungle"] = "Лес",
        ["Mid"] = "Центр",
        ["Roam"] = "Роум",
    };

    static readonly Dictionary<string, string> Emblems = new(StringComparer.OrdinalIgnoreCase)
    {
        ["All"] = "Универсальная эмблема",
        ["Assassin"] = "Эмблема убийцы",
        ["Fighter"] = "Эмблема бойца",
        ["Mage"] = "Эмблема мага",
        ["Marksman"] = "Эмблема стрелка",
        ["Support"] = "Эмблема поддержки",
        ["Tank"] = "Эмблема танка",
    };

    static readonly Dictionary<string, string> Spells = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Aegis"] = "Щит",
        ["Arrival"] = "Прибытие",
        ["Execute"] = "Казнь",
        ["Flameshot"] = "Огненный выстрел",
        ["Flicker"] = "Вспышка",
        ["Inspire"] = "Натиск",
        ["Petrify"] = "Окаменение",
        ["Purify"] = "Очищение",
        ["Retribution"] = "Возмездие",
        ["Revitalize"] = "Восстановление",
        ["Sprint"] = "Спринт",
        ["Vengeance"] = "Отмщение",
    };

    static readonly Dictionary<string, string> Talents = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Agility"] = "Ловкость",
        ["Bargain Hunter"] = "Выгодная покупка",
        ["Brave Smite"] = "Храбрый удар",
        ["Fatal"] = "Фатальность",
        ["Firmness"] = "Твёрдость",
        ["Concussive Blast"] = "Сотрясающий взрыв",
        ["Focusing Mark"] = "Метка концентрации",
        ["Festival of Blood"] = "Кровавый пир",
        ["Impure Rage"] = "Нечистая ярость",
        ["Inspire"] = "Воодушевление",
        ["Quantum Charge"] = "Квантовый заряд",
        ["Seasoned Hunter"] = "Опытный охотник",
        ["Temporal Reign"] = "Власть времени",
        ["Thrill"] = "Азарт",
        ["Wilderness Blessing"] = "Благословение дикой природы",
        ["Weapon Master"] = "Мастер оружия",
        ["War Cry"] = "Боевой клич",
        ["Killing Spree"] = "Серия убийств",
        ["Lethal Ignition"] = "Смертоносное воспламенение",
        ["Master Assassin"] = "Мастер убийства",
        ["Pull Yourself Together"] = "Соберись",
        ["Rupture"] = "Разлом",
        ["Swift"] = "Стремительность",
        ["Tenacity"] = "Стойкость",
        ["Vitality"] = "Живучесть",
        ["Weakness Finder"] = "Искатель слабостей",
        ["Weapons Master"] = "Мастер оружия",
    };

    static readonly Dictionary<string, string> HeroNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["aamon"] = "Эймон", ["akai"] = "Акай", ["aldous"] = "Олдос", ["alice"] = "Алиса",
        ["alpha"] = "Альфа", ["alucard"] = "Алукард", ["angela"] = "Ангела", ["argus"] = "Аргус",
        ["arlott"] = "Арлотт", ["atlas"] = "Атлас", ["aulus"] = "Аулус", ["aurora"] = "Аврора",
        ["badang"] = "Баданг", ["balmond"] = "Бальмонд", ["bane"] = "Бэйн", ["barats"] = "Баратс",
        ["baxia"] = "Баксия", ["beatrix"] = "Беатрис", ["belerick"] = "Белерик", ["benedetta"] = "Бенедетта",
        ["brody"] = "Броди", ["bruno"] = "Бруно", ["carmilla"] = "Кармилла", ["cecilion"] = "Сесилион",
        ["chang-e"] = "Чанъэ", ["chip"] = "Чип", ["chou"] = "Чу", ["cici"] = "Сиси", ["claude"] = "Клод",
        ["clint"] = "Клинт", ["cyclops"] = "Циклоп", ["diggie"] = "Дигги", ["dyrroth"] = "Диррот",
        ["edith"] = "Эдит", ["esmeralda"] = "Эсмеральда", ["estes"] = "Эстес", ["eudora"] = "Эудора",
        ["fanny"] = "Фанни", ["faramis"] = "Фарамис", ["floryn"] = "Флорин", ["franco"] = "Франко",
        ["fredrinn"] = "Фредрин", ["freya"] = "Фрея", ["gatotkaca"] = "Гатоткача", ["gloo"] = "Глу",
        ["gord"] = "Горд", ["granger"] = "Грейнджер", ["grock"] = "Грок", ["guinevere"] = "Гвиневра",
        ["gusion"] = "Гусион", ["hanabi"] = "Ханаби", ["hanzo"] = "Ханзо", ["harith"] = "Харит",
        ["harley"] = "Харли", ["hayabusa"] = "Хаябуса", ["helcurt"] = "Хелкерт", ["hilda"] = "Хильда",
        ["hirara"] = "Хирара", ["hylos"] = "Хилос", ["irithel"] = "Иритэль", ["ixia"] = "Иксия",
        ["jawhead"] = "Джаухед", ["johnson"] = "Джонсон", ["joy"] = "Джой", ["julian"] = "Джулиан",
        ["kadita"] = "Кадита", ["kagura"] = "Кагура", ["kaja"] = "Каджа", ["kalea"] = "Калеа",
        ["karina"] = "Карина", ["karrie"] = "Кэрри", ["khaleed"] = "Халид", ["khufra"] = "Хуфра",
        ["kimmy"] = "Кимми", ["lancelot"] = "Ланселот", ["lapu-lapu"] = "Лапу-Лапу", ["layla"] = "Лейла",
        ["leomord"] = "Леоморд", ["lesley"] = "Лесли", ["ling"] = "Линг", ["lolita"] = "Лолита",
        ["lukas"] = "Лукас", ["lunox"] = "Лунокс", ["luo-yi"] = "Ло И", ["lylia"] = "Лилия",
        ["marcel"] = "Марсель", ["martis"] = "Мартис", ["masha"] = "Маша", ["mathilda"] = "Матильда",
        ["melissa"] = "Мелисса", ["minotaur"] = "Минотавр", ["minsitthar"] = "Минситтар", ["miya"] = "Мия",
        ["moskov"] = "Москов", ["nana"] = "Нана", ["natalia"] = "Наталья", ["natan"] = "Натан",
        ["nolan"] = "Нолан", ["novaria"] = "Новария", ["obsidia"] = "Обсидия", ["odette"] = "Одетта",
        ["paquito"] = "Пакито", ["pharsa"] = "Фарса", ["phoveus"] = "Фовеус", ["popol-and-kupa"] = "Пополь и Купа",
        ["rafaela"] = "Рафаэла", ["roger"] = "Роджер", ["ruby"] = "Руби", ["saber"] = "Сэйбер",
        ["selena"] = "Селена", ["silvanna"] = "Сильванна", ["sora"] = "Сора", ["sun"] = "Сун",
        ["suyou"] = "Сую", ["terizla"] = "Теризла", ["thamuz"] = "Тамуз", ["tigreal"] = "Тигрил",
        ["uranus"] = "Уранус", ["vale"] = "Вэйл", ["valentina"] = "Валентина", ["valir"] = "Валир",
        ["vexana"] = "Вексана", ["wanwan"] = "Ванван", ["x-borg"] = "Икс Борг", ["xavier"] = "Ксавьер",
        ["yi-sun-shin"] = "И Сун-Шин", ["yin"] = "Инь", ["yu-zhong"] = "Юй Чжун", ["yve"] = "Ив",
        ["zetian"] = "Цзэтянь", ["zhask"] = "Жаск", ["zhuxin"] = "Чжусинь", ["zilong"] = "Зилонг",
    };

    public static string RoleName(string role) => Roles.TryGetValue(role, out var value) ? value : role;
    public static string LaneName(string lane) => Lanes.TryGetValue(lane, out var value) ? value : lane;
    public static string EmblemName(string name) => Emblems.TryGetValue(name, out var value) ? value : name;
    public static string SpellName(string name) => Spells.TryGetValue(name, out var value) ? value : name;
    public static string TalentName(string name) => Talents.TryGetValue(name, out var value) ? value : name;
    public static string HeroName(string id, string englishName) =>
        HeroNames.TryGetValue(id, out var value) ? value : Transliterate(englishName);

    static string Transliterate(string value)
    {
        var input = value.ToLowerInvariant();
        var output = new StringBuilder(input.Length * 2);
        var capitalizeNext = true;

        for (var index = 0; index < input.Length;)
        {
            if (char.IsWhiteSpace(input[index]) || input[index] is '-' or '\'' or '.')
            {
                output.Append(input[index]);
                capitalizeNext = true;
                index++;
                continue;
            }

            string? converted = null;
            foreach (var pair in LetterPairs)
            {
                if (input.AsSpan(index).StartsWith(pair.Latin, StringComparison.Ordinal))
                {
                    converted = pair.Cyrillic;
                    index += pair.Latin.Length;
                    break;
                }
            }

            if (converted is null)
            {
                converted = Letters.TryGetValue(input[index], out var letter)
                    ? letter
                    : input[index].ToString();
                index++;
            }

            if (capitalizeNext)
            {
                output.Append(converted[..1].ToUpperInvariant());
                output.Append(converted[1..]);
                capitalizeNext = false;
            }
            else
            {
                output.Append(converted);
            }
        }

        return output.ToString();
    }

    public static string RankName(string rank) => rank.ToLowerInvariant() switch
    {
        "all" => "все ранги",
        "epic" => "Эпик",
        "legend" => "Легенда",
        "mythic" => "Мифический",
        "honor" => "Мифическая честь",
        "glory" => "Мифическая слава",
        _ => rank
    };
}
