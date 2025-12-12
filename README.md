<h1 align="center">
    Outward Mod Pack Template
</h1>
<br/>
<div align="center">
  <img src="https://raw.githubusercontent.com/GymMed/Outward-Mod-Pack-Template/refs/heads/main/preview/images/Logo.png" alt="Logo"/>
</div>

<div align="center">
	A template for creating a C# Outward Mod.
</div>

## How to use

1. Either clone/download the repository with Git or GitHub Desktop, or simply download the code manually.
2. Open `src/OutwardModPackTemplate.sln` with any C# IDE (Visual Studio, Rider, etc)
3. Open the `OutwardModPackTemplate.cs` file and follow the instructions
4. When you're ready, build the solution. It will be built to the `Release` folder (next to the `src` folder).
5. Take the DLL from the `Release` folder and put it in the `BepInEx/plugins/` folder. If you use r2modman, this can be found by going into r2modman settings and clicking on `Browse Profile Folder`.

## Tips

### Faster testing setup

Place automatically `.dll` document to your `r2modman` profile for quicker testing.
Inside `src/OutwardModPackTemplate.csproj` and in `Project` tag you can add:

<pre><code>&lt;Target Name="PostBuild" AfterTargets="PostBuildEvent"&gt;
  &lt;Exec Command="call &amp;quot;$(ProjectDir)..\Public\DontZip\placeBuild.bat&amp;quot;" /&gt;
&lt;/Target&gt;</code></pre>

Make sure that you changed variables inside `..\Public\DontZip\placeBuild.bat`.

### Choose the right project size

This repository contains three project-size templates, each provided as its own branch

- **Small** – basic structure  
  *(example: [Outward-Scene-Tester](https://github.com/GymMed/Outward-Scene-Tester))*

- **Medium** – standard mod structure  
  *(example: [Outward-Enchantments-Viewer](https://github.com/GymMed/Outward-Enchantments-Viewer))*

- **Large** – expanded architecture for big mods  
  *(example: [Outward-Loot-Manager](https://github.com/GymMed/Outward-Loot-Manager))*

Each size supports a different way of organizing code:

- Use **Small** when learning the basics.  
- Use **Medium** for typical mods.  
- Use **Large** for big projects where you want to reduce code clutter and improve maintainability.

### Thunderstore Release

All the necessary documents can be found in the 
[./Public](https://github.com/GymMed/Outward-Mod-Pack-Template/tree/main/Public) directory.

**Important:**
- Do **not** include the files in [./Public/DontZip](https://github.com/GymMed/Outward-Mod-Pack-Template/tree/main/Public/DontZip) when packaging your mod.
- Feel free to edit the other files as needed.

For further help, see the [Outward Modding Wiki](https://outward.fandom.com/wiki/Getting_Started_Developing_Mods), or join the [Outward Modding Discord](https://discord.gg/zKyfGmy7TR).
