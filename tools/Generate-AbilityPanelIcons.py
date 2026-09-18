from __future__ import annotations

import json
import math
import shutil
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter, ImageFont


ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "src" / "GeneratedConfig" / "AbilityPanelIcons"
STYLE_REF = Path(
    r"C:\Users\Fran\.codex\generated_images\01a0afdf-af0f-7cf3-ace3-65ebab1c3e4b\call_IsVpf1o0HQBGAqrE8aRoVaI7.png"
)

ICON_SIZE = 128
SCALE = 4
W = ICON_SIZE * SCALE


THEMES = {
    "heimdall": {
        "bg": ((7, 23, 39), (25, 92, 140), (115, 210, 255)),
        "accent": (120, 225, 255),
        "hot": (245, 224, 116),
    },
    "ragnar": {
        "bg": ((36, 7, 8), (116, 24, 18), (248, 86, 36)),
        "accent": (245, 54, 50),
        "hot": (255, 186, 75),
    },
    "hraesvelgr": {
        "bg": ((7, 31, 18), (28, 101, 52), (122, 239, 143)),
        "accent": (136, 245, 142),
        "hot": (226, 255, 168),
    },
    "hellsyng": {
        "bg": ((20, 12, 34), (74, 32, 103), (170, 92, 226)),
        "accent": (187, 112, 255),
        "hot": (242, 236, 255),
    },
    "nott": {
        "bg": ((13, 10, 27), (49, 34, 92), (144, 92, 245)),
        "accent": (158, 109, 255),
        "hot": (92, 255, 199),
    },
    "seidr": {
        "bg": ((6, 28, 31), (16, 103, 110), (95, 241, 231)),
        "accent": (92, 238, 229),
        "hot": (179, 255, 244),
    },
    "helveig": {
        "bg": ((34, 6, 18), (110, 26, 52), (255, 92, 122)),
        "accent": (255, 92, 126),
        "hot": (255, 239, 202),
    },
    "moonvein": {
        "bg": ((9, 17, 43), (33, 61, 131), (99, 137, 255)),
        "accent": (135, 172, 255),
        "hot": (255, 205, 101),
    },
    "frostbrand": {
        "bg": ((7, 25, 42), (18, 90, 119), (255, 112, 62)),
        "accent": (111, 220, 255),
        "hot": (255, 117, 64),
    },
    "generic": {
        "bg": ((14, 17, 22), (54, 66, 78), (180, 206, 230)),
        "accent": (190, 220, 255),
        "hot": (246, 229, 160),
    },
    "pets": {
        "bg": ((14, 17, 22), (54, 66, 78), (180, 206, 230)),
        "accent": (190, 220, 255),
        "hot": (246, 229, 160),
    },
}


ICONS = [
    # Class buffs
    ("ClassBuffs", "heimdall_class_buff", "Heimdall Class Buff", "heimdall", "class_heimdall"),
    ("ClassBuffs", "ragnar_class_buff", "Ragnar Class Buff", "ragnar", "class_ragnar"),
    ("ClassBuffs", "hraesvelgr_class_buff", "Hraesvelgr Class Buff", "hraesvelgr", "class_hraesvelgr"),
    ("ClassBuffs", "hellsyng_class_buff", "Hellsyng Class Buff", "hellsyng", "class_hellsyng"),
    ("ClassBuffs", "nott_class_buff", "Nott Class Buff", "nott", "class_nott"),
    ("ClassBuffs", "seidr_class_buff", "Seidr Class Buff", "seidr", "class_seidr"),
    ("ClassBuffs", "helveig_class_buff", "Helveig Class Buff", "helveig", "class_helveig"),
    ("ClassBuffs", "moonvein_class_buff", "Moonvein Class Buff", "moonvein", "class_moonvein"),
    ("ClassBuffs", "frostbrand_class_buff", "Frostbrand Class Buff", "frostbrand", "class_frostbrand"),

    # Heimdall
    ("Abilities", "heimdall_lightning_storm", "Lightning Storm", "heimdall", "lightning_storm"),
    ("Abilities", "heimdall_stone_shield", "Stone Shield", "heimdall", "stone_shield"),
    ("Abilities", "heimdall_abyssal_harpoon", "Abyssal Harpoon", "heimdall", "harpoon"),
    ("Buffs", "heimdall_guard", "Heimdall Guard", "heimdall", "guard_stacks"),

    # Ragnar
    ("Abilities", "ragnar_decay_aura", "Decay Aura", "ragnar", "decay_aura"),
    ("Abilities", "ragnar_blood_frenzy", "Blood Frenzy", "ragnar", "blood_frenzy"),
    ("Abilities", "ragnar_crush", "Ragnar Crush", "ragnar", "crush"),
    ("Buffs", "ragnar_fury", "Ragnar Fury", "ragnar", "fury"),
    ("Buffs", "ragnar_blood_surge", "Blood Surge", "ragnar", "blood_surge"),
    ("Buffs", "ragnar_burning_ground", "Burning Ground", "ragnar", "burning_ground"),

    # Hraesvelgr
    ("Abilities", "hraesvelgr_summon_beasts", "Summon Beasts", "hraesvelgr", "summon_beasts"),
    ("Abilities", "hraesvelgr_armed_trap", "Armed Trap", "hraesvelgr", "trap"),
    ("Abilities", "hraesvelgr_rapid_volley", "Rapid Volley", "hraesvelgr", "rapid_volley"),
    ("Abilities", "hraesvelgr_freyja_dash", "Freyja Dash", "hraesvelgr", "dash"),
    ("Buffs", "hraesvelgr_sneaky", "Sneaky", "hraesvelgr", "sneaky"),
    ("Buffs", "hraesvelgr_headshot", "Headshot", "hraesvelgr", "headshot"),
    ("Buffs", "hraesvelgr_trap_charge", "Trap Charges", "hraesvelgr", "trap_charge"),
    ("Buffs", "hraesvelgr_trap_damage", "Trap Damage", "hraesvelgr", "trap_damage"),

    # Hellsyng
    ("Abilities", "hellsyng_pets", "Hellsyng Pets", "hellsyng", "hell_pets"),
    ("Abilities", "hellsyng_werewolf_form", "Werewolf Form", "hellsyng", "werewolf"),
    ("Abilities", "hellsyng_bat_form", "Bat Form", "hellsyng", "bat_form"),
    ("Abilities", "hellsyng_bat_horde", "Bat Horde", "hellsyng", "bat_horde"),
    ("Abilities", "hellsyng_rapid_fire", "Rapid Fire", "hellsyng", "rapid_fire"),
    ("Abilities", "hellsyng_bat_regeneration", "Bat Regeneration", "hellsyng", "bat_regen"),
    ("Abilities", "hellsyng_bite", "Bite", "hellsyng", "bite"),
    ("Abilities", "hellsyng_claw", "Claw", "hellsyng", "claw"),
    ("Buffs", "hellsyng_tnt", "T.N.T.", "hellsyng", "tnt"),
    ("Buffs", "hellsyng_silver_bullets", "Silver Bullets", "hellsyng", "silver_bullets"),
    ("Buffs", "hellsyng_witchmark", "Witchmark", "hellsyng", "witchmark"),
    ("Buffs", "hellsyng_silver_verdict", "Silver Verdict", "hellsyng", "silver_verdict"),
    ("Buffs", "hellsyng_hunt", "Hunt", "hellsyng", "hunt"),

    # Nott
    ("Abilities", "nott_warp", "Warp", "nott", "warp"),
    ("Abilities", "nott_shadow_mark", "Shadow Mark", "nott", "shadow_mark"),
    ("Abilities", "nott_knife_strike", "Knife Strike", "nott", "knife_strike"),
    ("Buffs", "nott_sneaky", "Sneaky", "nott", "sneaky"),
    ("Buffs", "nott_poison_edge", "Poison Edge", "nott", "poison_edge"),
    ("Buffs", "nott_executor", "Executor", "nott", "executor"),
    ("Buffs", "nott_shadow_momentum", "Shadow Momentum", "nott", "shadow_momentum"),
    ("Buffs", "nott_warp_strike", "Warp Strike", "nott", "warp_strike"),
    ("Buffs", "nott_warp_guard", "Warp Guard", "nott", "warp_guard"),

    # Seidr
    ("Abilities", "seidr_nanocube", "Nanocube", "seidr", "nanocube"),
    ("Abilities", "seidr_elemental_shield", "Elemental Shield", "seidr", "elemental_shield"),
    ("Abilities", "seidr_stone_golem", "Stone Golem", "seidr", "stone_golem"),
    ("Abilities", "seidr_frost_nova", "Frost Nova", "seidr", "frost_nova"),
    ("Buffs", "seidr_nanocube_power", "Nanocube Power", "seidr", "nanocube_power"),
    ("Buffs", "seidr_frost_nova_slow", "Frost Nova Slow", "seidr", "frost_slow"),

    # Helveig
    ("Abilities", "helveig_holy_heal", "Holy Heal", "helveig", "holy_heal"),
    ("Abilities", "helveig_blood_rite", "Blood Rite", "helveig", "blood_rite"),
    ("Abilities", "helveig_holy_strike", "Holy Strike", "helveig", "holy_strike"),
    ("Abilities", "helveig_summon_monster", "Summon Monster", "helveig", "summon_monster"),
    ("Buffs", "helveig_undead_bodyguard", "Undead Bodyguard", "helveig", "undead_bodyguard"),
    ("Buffs", "helveig_blood_aegis", "Blood Aegis", "helveig", "blood_aegis"),
    ("Buffs", "helveig_sanguine_devotion", "Sanguine Devotion", "helveig", "sanguine_devotion"),
    ("Buffs", "helveig_bodyguard_respawn", "Bodyguard Respawn", "helveig", "bodyguard_respawn"),

    # Moonvein
    ("Abilities", "moonvein_meteor", "Meteor", "moonvein", "meteor"),
    ("Abilities", "moonvein_tornado_shot", "Tornado Shot", "moonvein", "tornado_shot"),
    ("Buffs", "moonvein_charged_shots", "Charged Shots", "moonvein", "charged_shots"),
    ("Buffs", "moonvein_tornado_slow", "Tornado Slow", "moonvein", "tornado_slow"),

    # Frostbrand
    ("Abilities", "frostbrand_water_sphere", "Water Sphere", "frostbrand", "water_sphere"),
    ("Abilities", "frostbrand_slash", "Slash", "frostbrand", "slash"),
    ("Abilities", "frostbrand_elemental_shield", "Elemental Shield", "frostbrand", "elemental_shield"),
    ("Abilities", "frostbrand_lightning_strike", "Lightning Strike", "frostbrand", "lightning_strike"),
    ("Buffs", "frostbrand_fire_ball", "Fire Ball", "frostbrand", "fire_ball"),
    ("Buffs", "frostbrand_recharge", "Recharge", "frostbrand", "recharge"),
    ("Buffs", "frostbrand_burning_ground", "Burning Ground", "frostbrand", "burning_ground"),

    # Global pet commands
    ("Abilities", "pet_attack", "Pet Attack", "pets", "pet_attack"),
    ("Abilities", "pet_follow", "Pet Follow", "pets", "pet_follow"),
    ("Abilities", "pet_free", "Pet Free", "pets", "pet_free"),
]


def sc(v: float) -> int:
    return int(round(v * SCALE))


def rgba(color, alpha=255):
    return (int(color[0]), int(color[1]), int(color[2]), int(alpha))


def mix(a, b, t):
    return tuple(int(a[i] + (b[i] - a[i]) * t) for i in range(3))


def radial_background(theme):
    base, mid, high = theme["bg"]
    img = Image.new("RGBA", (W, W), (0, 0, 0, 0))
    pix = img.load()
    cx, cy = W * 0.45, W * 0.36
    for y in range(W):
        for x in range(W):
            dx = (x - cx) / W
            dy = (y - cy) / W
            d = min(1.0, math.sqrt(dx * dx + dy * dy) * 2.25)
            if d < 0.55:
                col = mix(high, mid, d / 0.55)
            else:
                col = mix(mid, base, (d - 0.55) / 0.45)
            pix[x, y] = (*col, 255)
    return img


def rounded_mask(rect, radius):
    mask = Image.new("L", (W, W), 0)
    d = ImageDraw.Draw(mask)
    d.rounded_rectangle(tuple(sc(v) for v in rect), radius=sc(radius), fill=255)
    return mask


def create_base(theme):
    img = Image.new("RGBA", (W, W), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    shadow = Image.new("RGBA", (W, W), (0, 0, 0, 0))
    sd = ImageDraw.Draw(shadow)
    sd.rounded_rectangle((sc(6), sc(8), sc(122), sc(124)), radius=sc(17), fill=(0, 0, 0, 180))
    shadow = shadow.filter(ImageFilter.GaussianBlur(sc(3)))
    img.alpha_composite(shadow)

    draw.rounded_rectangle((sc(7), sc(6), sc(121), sc(120)), radius=sc(16), fill=(20, 22, 25, 255))
    draw.rounded_rectangle((sc(10), sc(9), sc(118), sc(117)), radius=sc(14), outline=(95, 97, 92, 255), width=sc(2))
    draw.rounded_rectangle((sc(15), sc(14), sc(113), sc(112)), radius=sc(11), fill=(7, 9, 12, 255))

    inner = radial_background(theme)
    mask = rounded_mask((17, 16, 111, 110), 10)
    img.alpha_composite(Image.composite(inner, Image.new("RGBA", (W, W), (0, 0, 0, 0)), mask))
    draw.rounded_rectangle((sc(17), sc(16), sc(111), sc(110)), radius=sc(10), outline=(5, 6, 8, 210), width=sc(2))
    draw.rounded_rectangle((sc(18), sc(17), sc(110), sc(109)), radius=sc(9), outline=rgba(theme["accent"], 90), width=sc(1))

    # Small decorative rivets.
    for x, y in ((16, 15), (112, 15), (16, 111), (112, 111)):
        draw.ellipse((sc(x - 3), sc(y - 3), sc(x + 3), sc(y + 3)), fill=(58, 60, 61, 255), outline=(8, 8, 8, 255), width=sc(1))
    return img


def layer_glow(img, func, color, blur=4, alpha=150):
    glow = Image.new("RGBA", (W, W), (0, 0, 0, 0))
    gd = ImageDraw.Draw(glow)
    func(gd, rgba(color, alpha))
    glow = glow.filter(ImageFilter.GaussianBlur(sc(blur)))
    img.alpha_composite(glow)
    d = ImageDraw.Draw(img)
    func(d, rgba(color, 245))


def line(draw, pts, fill, width=4, joint="curve"):
    draw.line([(sc(x), sc(y)) for x, y in pts], fill=fill, width=sc(width), joint=joint)


def poly(draw, pts, fill, outline=None, width=2):
    draw.polygon([(sc(x), sc(y)) for x, y in pts], fill=fill)
    if outline:
        line(draw, pts + [pts[0]], outline, width)


def ellipse(draw, box, fill=None, outline=None, width=2):
    draw.ellipse(tuple(sc(v) for v in box), fill=fill, outline=outline, width=sc(width))


def arc(draw, box, start, end, fill, width=4):
    draw.arc(tuple(sc(v) for v in box), start=start, end=end, fill=fill, width=sc(width))


def flame_shape(x, y, scale=1.0):
    return [
        (x, y - 34 * scale),
        (x + 15 * scale, y - 13 * scale),
        (x + 8 * scale, y + 19 * scale),
        (x - 1 * scale, y + 30 * scale),
        (x - 16 * scale, y + 13 * scale),
        (x - 10 * scale, y - 7 * scale),
    ]


def bolt_shape(x=65, y=62, scale=1.0):
    return [
        (x - 9 * scale, y - 42 * scale),
        (x + 11 * scale, y - 14 * scale),
        (x + 1 * scale, y - 13 * scale),
        (x + 17 * scale, y + 38 * scale),
        (x - 15 * scale, y + 3 * scale),
        (x - 3 * scale, y + 2 * scale),
    ]


def shield_points(x=64, y=63, w=42, h=58):
    return [(x, y - h / 2), (x + w / 2, y - h / 4), (x + w * 0.38, y + h * 0.25), (x, y + h / 2), (x - w * 0.38, y + h * 0.25), (x - w / 2, y - h / 4)]


def draw_arrow(draw, start, end, color, width=4):
    sx, sy = start
    ex, ey = end
    line(draw, [start, end], color, width)
    angle = math.atan2(ey - sy, ex - sx)
    size = 11
    left = (ex - math.cos(angle - 0.65) * size, ey - math.sin(angle - 0.65) * size)
    right = (ex - math.cos(angle + 0.65) * size, ey - math.sin(angle + 0.65) * size)
    poly(draw, [end, left, right], color)


def draw_bat(draw, cx, cy, scale, color):
    pts = [
        (cx, cy - 6 * scale),
        (cx + 9 * scale, cy - 12 * scale),
        (cx + 26 * scale, cy - 5 * scale),
        (cx + 14 * scale, cy + 2 * scale),
        (cx + 22 * scale, cy + 11 * scale),
        (cx + 5 * scale, cy + 7 * scale),
        (cx, cy + 16 * scale),
        (cx - 5 * scale, cy + 7 * scale),
        (cx - 22 * scale, cy + 11 * scale),
        (cx - 14 * scale, cy + 2 * scale),
        (cx - 26 * scale, cy - 5 * scale),
        (cx - 9 * scale, cy - 12 * scale),
    ]
    poly(draw, pts, color)


def draw_paw(draw, cx, cy, color, scale=1.0):
    ellipse(draw, (cx - 12 * scale, cy - 2 * scale, cx + 12 * scale, cy + 20 * scale), fill=color)
    for dx, dy, r in [(-16, -12, 6), (-5, -18, 6), (7, -18, 6), (18, -11, 6)]:
        ellipse(draw, (cx + dx * scale - r * scale, cy + dy * scale - r * scale, cx + dx * scale + r * scale, cy + dy * scale + r * scale), fill=color)


def draw_snowflake(draw, cx, cy, color, scale=1.0):
    for a in range(0, 180, 30):
        r = math.radians(a)
        dx = math.cos(r) * 32 * scale
        dy = math.sin(r) * 32 * scale
        line(draw, [(cx - dx, cy - dy), (cx + dx, cy + dy)], color, 3)
    ellipse(draw, (cx - 8 * scale, cy - 8 * scale, cx + 8 * scale, cy + 8 * scale), fill=color)


def draw_cross(draw, cx, cy, color, scale=1.0):
    poly(draw, [
        (cx - 6 * scale, cy - 32 * scale),
        (cx + 6 * scale, cy - 32 * scale),
        (cx + 6 * scale, cy - 8 * scale),
        (cx + 26 * scale, cy - 8 * scale),
        (cx + 26 * scale, cy + 5 * scale),
        (cx + 6 * scale, cy + 5 * scale),
        (cx + 6 * scale, cy + 34 * scale),
        (cx - 6 * scale, cy + 34 * scale),
        (cx - 6 * scale, cy + 5 * scale),
        (cx - 26 * scale, cy + 5 * scale),
        (cx - 26 * scale, cy - 8 * scale),
        (cx - 6 * scale, cy - 8 * scale),
    ], color)


def draw_symbol(img, kind, theme_name):
    theme = THEMES[theme_name]
    accent = theme["accent"]
    hot = theme["hot"]
    white = (245, 248, 246)
    d = ImageDraw.Draw(img)

    def glow(draw_func, color=accent, blur=4, alpha=145):
        layer_glow(img, draw_func, color, blur, alpha)

    if kind == "pet_attack":
        glow(lambda g, c: draw_paw(g, 54, 69, c, 0.88), accent, 5)
        draw_arrow(d, (42, 91), (91, 42), rgba(hot, 240), 5)

    elif kind == "pet_follow":
        glow(lambda g, c: draw_paw(g, 55, 70, c, 0.86), accent, 5)
        arc(d, (35, 30, 98, 96), 210, 500, rgba(hot, 230), 5)
        draw_arrow(d, (88, 43), (99, 55), rgba(hot, 235), 4)

    elif kind == "pet_free":
        glow(lambda g, c: draw_paw(g, 64, 69, c, 0.9), accent, 5)
        for x, y in ((39, 42), (86, 38), (94, 82), (35, 88)):
            ellipse(d, (x - 4, y - 4, x + 4, y + 4), fill=rgba(hot, 235))
        arc(d, (31, 31, 97, 97), 28, 148, rgba(white, 210), 4)
        arc(d, (31, 31, 97, 97), 208, 328, rgba(white, 210), 4)

    elif kind in ("lightning_storm", "lightning_strike"):
        for x in (44, 66, 85) if kind == "lightning_strike" else (64,):
            glow(lambda g, c, x=x: poly(g, bolt_shape(x, 63, 0.9 if kind == "lightning_strike" else 1.15), c), hot if kind == "lightning_strike" else accent, 5)
        if kind == "lightning_storm":
            arc(d, (28, 25, 100, 104), 210, 330, rgba(accent, 210), 4)
            line(d, [(31, 89), (97, 89)], rgba(white, 180), 2)

    elif kind == "stone_shield":
        glow(lambda g, c: poly(g, shield_points(64, 63, 55, 68), rgba((176, 178, 169), 245), outline=rgba(hot, 230), width=3), hot, 4)
        line(d, [(64, 30), (64, 95)], (63, 65, 65, 255), 3)
        line(d, [(39, 52), (89, 52)], (63, 65, 65, 255), 3)
        ellipse(d, (54, 48, 74, 68), fill=(42, 42, 42, 255), outline=rgba(hot, 200), width=2)

    elif kind == "harpoon":
        glow(lambda g, c: draw_arrow(g, (28, 88), (91, 32), c, 5), accent, 4)
        poly(d, [(91, 32), (106, 22), (100, 45)], rgba(white, 240), outline=rgba(accent, 230), width=2)
        line(d, [(28, 89), (43, 95), (58, 91), (73, 99)], rgba(hot, 180), 2)

    elif kind == "guard_stacks":
        for off in (-12, 0, 12):
            poly(d, [(64 + off, 31), (83 + off, 45), (77 + off, 84), (64 + off, 97), (51 + off, 84), (45 + off, 45)], rgba(accent, 105), outline=rgba(white, 210), width=2)
        glow(lambda g, c: poly(g, shield_points(64, 63, 40, 54), c), hot, 5)

    elif kind == "decay_aura":
        for r, a in [(37, 70), (28, 120), (18, 170)]:
            ellipse(d, (64 - r, 64 - r, 64 + r, 64 + r), outline=rgba((89, 225, 82), a), width=3)
        glow(lambda g, c: poly(g, [(64, 36), (77, 61), (67, 61), (76, 91), (52, 63), (62, 63)], c), (130, 255, 100), 5)

    elif kind == "blood_frenzy":
        glow(lambda g, c: poly(g, flame_shape(64, 66, 1.15), c), accent, 5)
        for x in (49, 64, 79):
            line(d, [(x, 88), (x + 8, 38)], rgba(white, 220), 3)

    elif kind in ("crush", "burning_ground"):
        if kind == "crush":
            poly(d, [(48, 33), (80, 33), (87, 47), (41, 47)], rgba((130, 130, 122), 245), outline=rgba(hot, 210), width=2)
            line(d, [(64, 45), (64, 82)], rgba(white, 220), 5)
            glow(lambda g, c: poly(g, [(31, 93), (51, 78), (64, 98), (78, 78), (99, 93)], c), hot, 5)
        else:
            glow(lambda g, c: poly(g, flame_shape(44, 78, 0.75), c), hot, 4)
            glow(lambda g, c: poly(g, flame_shape(66, 72, 1.0), c), accent, 5)
            glow(lambda g, c: poly(g, flame_shape(88, 80, 0.7), c), hot, 3)
            line(d, [(28, 99), (101, 99)], rgba((70, 28, 14), 230), 6)

    elif kind == "fury":
        glow(lambda g, c: poly(g, flame_shape(64, 65, 1.15), c), accent, 5)
        for a in (-22, 0, 22):
            line(d, [(64, 67), (64 + a, 34)], rgba(white, 210), 3)

    elif kind == "blood_surge":
        glow(lambda g, c: ellipse(g, (37, 35, 91, 90), fill=rgba(accent, 220)), accent, 5)
        poly(d, [(64, 101), (39, 63), (49, 42), (64, 52), (80, 42), (90, 63)], rgba(accent, 245), outline=rgba(white, 210), width=2)
        line(d, [(46, 67), (59, 67), (64, 56), (70, 79), (77, 67), (90, 67)], rgba(white, 230), 3)

    elif kind in ("summon_beasts", "hell_pets"):
        glow(lambda g, c: draw_paw(g, 54, 68, c, 0.95), accent, 5)
        if kind == "hell_pets":
            draw_bat(d, 82, 43, 0.55, rgba(hot, 230))
        else:
            arc(d, (29, 29, 99, 100), 215, 325, rgba(hot, 220), 4)

    elif kind in ("trap", "trap_charge", "trap_damage"):
        col = rgba(hot if kind == "trap_damage" else accent, 230)
        line(d, [(35, 88), (53, 49), (64, 84), (76, 49), (94, 88)], col, 5)
        line(d, [(35, 88), (94, 88)], rgba(white, 210), 3)
        if kind == "trap_charge":
            for x in (49, 64, 79):
                ellipse(d, (x - 4, 33, x + 4, 41), fill=rgba(hot, 235))
        if kind == "trap_damage":
            draw_arrow(d, (38, 38), (90, 82), rgba(hot, 240), 4)

    elif kind in ("rapid_volley", "charged_shots"):
        starts = [(29, 84), (26, 65), (32, 47)] if kind == "rapid_volley" else [(33, 80), (31, 64), (33, 48)]
        for i, start in enumerate(starts):
            draw_arrow(d, start, (92, start[1] - 24 + i * 8), rgba(accent if i < 2 else hot, 245), 4)
        if kind == "charged_shots":
            ellipse(d, (52, 50, 76, 74), outline=rgba(hot, 220), width=3)

    elif kind == "dash":
        glow(lambda g, c: draw_arrow(g, (30, 75), (88, 44), c, 6), accent, 5)
        for y in (56, 70, 84):
            line(d, [(30, y), (59, y - 10)], rgba(white, 130), 2)

    elif kind == "headshot":
        ellipse(d, (38, 36, 90, 88), outline=rgba(white, 230), width=4)
        line(d, [(64, 35), (64, 89)], rgba(white, 180), 2)
        line(d, [(37, 62), (91, 62)], rgba(white, 180), 2)
        draw_arrow(d, (24, 94), (78, 48), rgba(accent, 245), 4)

    elif kind == "sneaky":
        poly(d, [(35, 75), (46, 39), (64, 29), (82, 39), (93, 75), (76, 98), (52, 98)], rgba((18, 17, 27), 245), outline=rgba(accent, 190), width=2)
        ellipse(d, (49, 58, 79, 74), fill=rgba(accent, 210))
        ellipse(d, (58, 62, 70, 70), fill=(7, 7, 12, 255))

    elif kind == "werewolf":
        poly(d, [(64, 25), (83, 41), (93, 70), (80, 100), (64, 90), (48, 100), (35, 70), (45, 41)], rgba((34, 37, 46), 245), outline=rgba(white, 210), width=2)
        poly(d, [(45, 40), (38, 24), (57, 34)], rgba((34, 37, 46), 245))
        poly(d, [(83, 40), (90, 24), (71, 34)], rgba((34, 37, 46), 245))
        ellipse(d, (51, 57, 57, 63), fill=rgba(accent, 255))
        ellipse(d, (71, 57, 77, 63), fill=rgba(accent, 255))
        line(d, [(52, 82), (64, 92), (76, 82)], rgba(hot, 230), 3)

    elif kind in ("bat_form", "bat_horde"):
        positions = [(64, 62, 1.1)] if kind == "bat_form" else [(46, 52, 0.55), (76, 44, 0.65), (66, 78, 0.75), (94, 70, 0.48)]
        for cx, cy, s in positions:
            glow(lambda g, c, cx=cx, cy=cy, s=s: draw_bat(g, cx, cy, s, c), accent, 4)

    elif kind == "rapid_fire":
        for y in (44, 61, 78):
            draw_arrow(d, (31, y), (90, y - 8), rgba(white, 235), 4)
            poly(d, flame_shape(97, y - 9, 0.42), rgba(hot, 240))

    elif kind == "bat_regen":
        draw_bat(d, 64, 53, 0.9, rgba(accent, 230))
        draw_cross(d, 64, 78, rgba((118, 255, 127), 235), 0.58)

    elif kind == "bite":
        poly(d, [(43, 34), (56, 79), (64, 41), (72, 79), (85, 34), (78, 95), (64, 105), (50, 95)], rgba(white, 240), outline=rgba(accent, 210), width=2)

    elif kind == "claw":
        for x in (47, 64, 81):
            line(d, [(x + 13, 30), (x - 12, 96)], rgba(white, 230), 7)
            line(d, [(x + 13, 30), (x - 12, 96)], rgba(accent, 230), 3)

    elif kind in ("tnt", "silver_bullets", "witchmark", "silver_verdict", "hunt"):
        if kind == "tnt":
            glow(lambda g, c: poly(g, flame_shape(64, 70, 1.05), c), hot, 5)
            ellipse(d, (48, 55, 80, 87), fill=(28, 18, 18, 255), outline=rgba(white, 220), width=3)
            line(d, [(64, 55), (77, 38)], rgba(hot, 240), 3)
        elif kind == "silver_bullets":
            for y in (44, 62, 80):
                draw_arrow(d, (35, y), (87, y), rgba((225, 230, 235), 245), 4)
        elif kind == "witchmark":
            ellipse(d, (37, 44, 91, 82), outline=rgba(accent, 230), width=4)
            poly(d, [(64, 50), (74, 64), (64, 78), (54, 64)], rgba(accent, 230))
        elif kind == "silver_verdict":
            draw_cross(d, 64, 61, rgba((232, 235, 240), 240), 0.9)
            draw_arrow(d, (30, 93), (94, 31), rgba(hot, 220), 3)
        else:
            poly(d, [(64, 31), (83, 47), (88, 75), (64, 98), (40, 75), (45, 47)], rgba((40, 42, 49), 240), outline=rgba(accent, 220), width=2)
            ellipse(d, (51, 55, 58, 62), fill=rgba(hot, 240))
            ellipse(d, (70, 55, 77, 62), fill=rgba(hot, 240))

    elif kind == "warp":
        for r, a in [(35, 220), (26, 160), (16, 120)]:
            arc(d, (64 - r, 64 - r, 64 + r, 64 + r), 35, 335, rgba(accent, a), 5)
        draw_arrow(d, (42, 83), (83, 43), rgba(hot, 230), 4)

    elif kind == "shadow_mark":
        ellipse(d, (31, 43, 97, 83), outline=rgba(accent, 230), width=5)
        ellipse(d, (54, 51, 74, 75), fill=rgba(accent, 230))
        poly(d, [(64, 29), (71, 43), (57, 43)], rgba(hot, 220))

    elif kind == "poison_edge":
        draw_arrow(d, (36, 92), (86, 38), rgba(white, 230), 5)
        ellipse(d, (62, 64, 94, 96), fill=rgba((95, 255, 96), 130), outline=rgba((95, 255, 96), 230), width=3)

    elif kind == "shadow_momentum":
        for y in (47, 64, 81):
            line(d, [(34, y), (74, y - 16)], rgba(accent, 170), 5)
        draw_arrow(d, (48, 91), (91, 47), rgba(hot, 225), 4)

    elif kind == "warp_strike":
        arc(d, (29, 29, 99, 99), 40, 320, rgba(accent, 210), 5)
        draw_arrow(d, (41, 88), (88, 40), rgba(white, 240), 6)

    elif kind in ("nanocube", "nanocube_power"):
        front = [(48, 50), (76, 42), (92, 64), (64, 75)]
        side = [(48, 50), (64, 75), (64, 101), (43, 78)]
        top = [(64, 75), (92, 64), (91, 91), (64, 101)]
        for pts, a in ((front, 165), (side, 115), (top, 135)):
            poly(d, pts, rgba(accent, a), outline=rgba(white, 185), width=2)
        if kind == "nanocube_power":
            ellipse(d, (31, 31, 97, 97), outline=rgba(hot, 170), width=4)

    elif kind == "elemental_shield":
        poly(d, shield_points(64, 63, 48, 62), rgba((25, 43, 51), 225), outline=rgba(accent, 230), width=3)
        poly(d, flame_shape(57, 62, 0.42), rgba(hot, 230))
        draw_snowflake(d, 73, 62, rgba((145, 230, 255), 230), 0.35)
        poly(d, bolt_shape(64, 76, 0.38), rgba((245, 235, 95), 230))

    elif kind == "stone_golem":
        for box in [(42, 42, 60, 65), (61, 35, 82, 63), (75, 56, 95, 82), (48, 66, 80, 96)]:
            d.rounded_rectangle(tuple(sc(v) for v in box), radius=sc(4), fill=(103, 107, 101, 245), outline=rgba(accent, 150), width=sc(2))
        ellipse(d, (58, 57, 65, 64), fill=rgba(hot, 230))
        ellipse(d, (72, 57, 79, 64), fill=rgba(hot, 230))

    elif kind in ("frost_nova", "frost_slow"):
        draw_snowflake(d, 64, 64, rgba(accent, 245), 1.0)
        if kind == "frost_slow":
            line(d, [(39, 91), (88, 91)], rgba(white, 230), 5)

    elif kind == "holy_heal":
        glow(lambda g, c: draw_cross(g, 64, 64, c, 0.95), hot, 5)
        ellipse(d, (31, 32, 97, 98), outline=rgba(accent, 150), width=3)

    elif kind == "blood_rite":
        ellipse(d, (32, 33, 96, 96), outline=rgba(accent, 210), width=5)
        poly(d, [(49, 42), (79, 42), (74, 72), (64, 88), (54, 72)], rgba((160, 18, 42), 220), outline=rgba(hot, 210), width=2)
        line(d, [(53, 50), (75, 50)], rgba(white, 180), 2)

    elif kind == "holy_strike":
        draw_cross(d, 64, 61, rgba(hot, 240), 0.85)
        draw_arrow(d, (34, 94), (91, 35), rgba(white, 230), 5)

    elif kind == "blood_aegis":
        poly(d, shield_points(64, 66, 50, 64), rgba((75, 18, 34), 232), outline=rgba(accent, 230), width=3)
        glow(lambda g, c: draw_cross(g, 64, 59, c, 0.58), hot, 4)
        ellipse(d, (51, 78, 77, 101), fill=rgba((170, 20, 44), 225), outline=rgba(white, 180), width=2)

    elif kind == "sanguine_devotion":
        ellipse(d, (36, 32, 92, 91), outline=rgba(accent, 220), width=4)
        poly(d, [(49, 45), (79, 45), (74, 70), (64, 88), (54, 70)], rgba((160, 18, 42), 230), outline=rgba(hot, 220), width=2)
        draw_cross(d, 64, 59, rgba(white, 230), 0.42)
        for cx in (45, 64, 83):
            ellipse(d, (cx - 4, 96, cx + 4, 104), fill=rgba(hot, 235))

    elif kind in ("summon_monster", "undead_bodyguard", "bodyguard_respawn"):
        if kind == "bodyguard_respawn":
            ellipse(d, (43, 37, 85, 79), fill=rgba((210, 213, 205), 230), outline=rgba(accent, 210), width=2)
            line(d, [(48, 91), (80, 91)], rgba(hot, 230), 4)
            arc(d, (36, 30, 92, 100), 230, 500, rgba(hot, 220), 4)
        elif kind == "undead_bodyguard":
            poly(d, shield_points(64, 67, 50, 63), rgba((72, 67, 70), 230), outline=rgba(accent, 220), width=3)
            ellipse(d, (51, 36, 77, 62), fill=rgba((220, 218, 210), 230))
            ellipse(d, (56, 46, 61, 51), fill=(12, 10, 10, 255))
            ellipse(d, (67, 46, 72, 51), fill=(12, 10, 10, 255))
        else:
            line(d, [(38, 89), (53, 67), (61, 84), (72, 55), (81, 82), (95, 64)], rgba(accent, 240), 6)
            line(d, [(32, 94), (98, 94)], rgba(hot, 210), 4)

    elif kind == "meteor":
        for cx, cy, s in [(77, 47, 0.7), (57, 62, 0.55), (83, 76, 0.45)]:
            glow(lambda g, c, cx=cx, cy=cy, s=s: poly(g, flame_shape(cx, cy, s), c), hot, 4)
            ellipse(d, (cx - 8 * s, cy + 2 * s, cx + 9 * s, cy + 19 * s), fill=(78, 52, 43, 255), outline=rgba(white, 170), width=2)

    elif kind in ("tornado_shot", "tornado_slow"):
        for r, a in [(32, 210), (23, 170), (14, 130)]:
            arc(d, (64 - r, 64 - r, 64 + r, 64 + r), 20, 325, rgba(accent, a), 5)
        draw_arrow(d, (33, 88), (91, 42), rgba(hot, 225), 3)
        if kind == "tornado_slow":
            draw_snowflake(d, 45, 43, rgba(white, 200), 0.35)

    elif kind == "water_sphere":
        glow(lambda g, c: ellipse(g, (34, 33, 94, 93), fill=rgba(accent, 170), outline=rgba(white, 220), width=3), accent, 5)
        arc(d, (40, 43, 93, 93), 180, 350, rgba(white, 210), 4)
        arc(d, (28, 51, 88, 100), 205, 345, rgba((130, 240, 255), 210), 3)

    elif kind == "slash":
        glow(lambda g, c: arc(g, (27, 26, 104, 104), 210, 335, c, 9), hot, 5)
        line(d, [(38, 94), (91, 37)], rgba(white, 235), 5)

    elif kind == "fire_ball":
        glow(lambda g, c: poly(g, flame_shape(64, 65, 1.15), c), hot, 5)
        ellipse(d, (48, 48, 80, 80), fill=rgba((255, 183, 82), 230), outline=rgba(white, 185), width=2)

    elif kind == "recharge":
        for y in (42, 61, 80):
            d.rounded_rectangle((sc(38), sc(y - 7), sc(90), sc(y + 7)), radius=sc(4), fill=rgba(accent, 110), outline=rgba(white, 190), width=sc(2))
            poly(d, bolt_shape(64, y, 0.28), rgba(hot, 230))

    elif kind.startswith("class_"):
        class_kind = kind.split("_", 1)[1]
        if class_kind == "heimdall":
            draw_symbol(img, "stone_shield", "heimdall")
            draw_symbol(img, "lightning_storm", "heimdall")
        elif class_kind == "ragnar":
            draw_arrow(d, (43, 91), (79, 36), rgba(white, 225), 7)
            poly(d, flame_shape(70, 66, 0.8), rgba(accent, 220))
        elif class_kind == "hraesvelgr":
            draw_symbol(img, "rapid_volley", "hraesvelgr")
        elif class_kind == "hellsyng":
            draw_symbol(img, "bat_form", "hellsyng")
            draw_arrow(d, (35, 91), (88, 42), rgba(white, 220), 4)
        elif class_kind == "nott":
            draw_symbol(img, "warp_strike", "nott")
        elif class_kind == "seidr":
            draw_symbol(img, "nanocube", "seidr")
        elif class_kind == "helveig":
            draw_symbol(img, "blood_rite", "helveig")
        elif class_kind == "moonvein":
            draw_symbol(img, "tornado_shot", "moonvein")
        elif class_kind == "frostbrand":
            draw_symbol(img, "slash", "frostbrand")
            draw_symbol(img, "water_sphere", "frostbrand")

    else:
        glow(lambda g, c: ellipse(g, (38, 38, 90, 90), fill=rgba(accent, 200)), accent, 4)


def make_icon(theme_name, kind):
    theme = THEMES.get(theme_name, THEMES["generic"])
    img = create_base(theme)
    draw_symbol(img, kind, theme_name)
    img = img.resize((ICON_SIZE, ICON_SIZE), Image.Resampling.LANCZOS)
    return img


def make_contact_sheet(records, title, path, columns=6):
    thumb = 96
    label_h = 34
    pad = 18
    title_h = 44
    rows = math.ceil(len(records) / columns)
    width = columns * (thumb + pad) + pad
    height = title_h + rows * (thumb + label_h + pad) + pad
    sheet = Image.new("RGBA", (width, height), (18, 19, 23, 255))
    draw = ImageDraw.Draw(sheet)
    font_title, font_label = load_fonts()
    draw.text((pad, 10), title, fill=(238, 229, 196, 255), font=font_title)
    for i, rec in enumerate(records):
        col = i % columns
        row = i // columns
        x = pad + col * (thumb + pad)
        y = title_h + row * (thumb + label_h + pad)
        icon = Image.open(rec["path"]).convert("RGBA").resize((thumb, thumb), Image.Resampling.LANCZOS)
        sheet.alpha_composite(icon, (x, y))
        label = rec["label"]
        if len(label) > 18:
            label = label[:17] + "."
        bbox = draw.textbbox((0, 0), label, font=font_label)
        tx = x + (thumb - (bbox[2] - bbox[0])) // 2
        draw.text((tx, y + thumb + 4), label, fill=(218, 221, 226, 255), font=font_label)
    path.parent.mkdir(parents=True, exist_ok=True)
    sheet.convert("RGB").save(path, quality=95)


def load_fonts():
    candidates = [
        Path(r"C:\Windows\Fonts\arial.ttf"),
        Path(r"C:\Windows\Fonts\segoeui.ttf"),
    ]
    for cand in candidates:
        if cand.exists():
            return ImageFont.truetype(str(cand), 22), ImageFont.truetype(str(cand), 11)
    return ImageFont.load_default(), ImageFont.load_default()


def main():
    if OUT.exists():
        shutil.rmtree(OUT)
    (OUT / "contact_sheets").mkdir(parents=True, exist_ok=True)
    if STYLE_REF.exists():
        shutil.copy2(STYLE_REF, OUT / "contact_sheets" / "style_reference_imagegen.png")

    records = []
    for category, file_stem, label, theme, kind in ICONS:
        icon = make_icon(theme, kind)
        folder = OUT / category
        folder.mkdir(parents=True, exist_ok=True)
        path = folder / f"{file_stem}.png"
        icon.save(path)
        records.append({
            "category": category,
            "id": file_stem,
            "label": label,
            "class": theme,
            "kind": kind,
            "path": str(path),
        })

    with (OUT / "manifest.json").open("w", encoding="utf-8") as f:
        json.dump(records, f, indent=2)
        f.write("\n")

    make_contact_sheet(records, "EpicLootRaritySets ability panel icons - all", OUT / "contact_sheets" / "contact_sheet_all.png", columns=7)
    for category in ("ClassBuffs", "Abilities", "Buffs"):
        subset = [r for r in records if r["category"] == category]
        make_contact_sheet(subset, f"EpicLootRaritySets - {category}", OUT / "contact_sheets" / f"contact_sheet_{category.lower()}.png", columns=6)

    print(f"Generated {len(records)} icons in {OUT}")
    print(OUT / "contact_sheets" / "contact_sheet_all.png")


if __name__ == "__main__":
    main()
