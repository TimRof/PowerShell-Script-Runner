# PowerShell Script Runner

## Overview

PowerShell Script Runner is a WinForms application designed to execute PowerShell scripts with dynamically generated user input forms based on script parameters. It simplifies the process of running scripts by automatically detecting required parameters and providing an intuitive interface.

## Installation & Setup

1. **Place Scripts**: Add your PowerShell scripts to the `Scripts` folder.
2. **Define Parameters**: Ensure each script includes a parameter block at the top using the `param` keyword:
   ```powershell
   param (
       [string]$Name,
       [switch]$Hidden,
       [datetime]$StartDate
   )
   ```
3. **Set Dependencies (Optional)**: You can make parameters visually dependent on others by adding a `#DependsOn:` comment above them. This will disable dependent controls in the application but will still pass the parameters to the script.
   ```powershell
   param (
       [string]$Name,
       [switch]$Hidden,
       [datetime]$StartDate #DependsOn: Hidden
   )
   ```
4. **Set Defaults (Optional)**: You can set defaults by adding behind the parameter name:
   ```powershell
   param (
       [string]$Name = "Default name",
       [switch]$Hidden,
       [datetime]$StartDate = "01-01-2000" #DependsOn: Hidden
   )
   ```

## Usage

1. Run `PowerShell Script Runner.exe`.
2. Select a script from the list.
3. Edit parameters as desired.
4. Click **Run Script** to execute the script.

## Notes

- Ensure scripts are structured correctly with `param ()` blocks for parameter detection.
- Parameter dependencies affect the UI only; all parameters are still passed to the script.
- Run application in Administrator mode to run scripts in elevated mode.
- Scripts must be compatible with the PowerShell environment on the host machine.
- Application might have to be run once for Script folder to appear, otherwise create it manually.