# FileX

FileX is an ASP.NET Core application designed to track file system changes by taking and comparing snapshots of a directory.

## Features

- **Snapshot Generation**: Scan a root directory to create a snapshot of its current state.
- **Change Tracking**: Compare the current state against previous snapshots to identify:
    - **New**: Files added since the last scan.
    - **Modified**: Files that have changed (based on hash).
    - **Deleted**: Files that no longer exist in the current scan.
- **Persistence**: Saves snapshots locally for historical tracking.
- **Directory Tracking**: Monitors subdirectory structures to detect additions and removals.

## Technical Details

FileX employs an iterative directory traversal approach to scan file systems. When a snapshot is generated, the application:

- **Hashing**: Computes a SHA256 hash for each file to ensure accurate change detection. Hashing is performed asynchronously to optimize performance.
- **Data Capture**: Records the file's relative path, size in bytes, and last modification timestamp (`LastModifiedUtc`) to establish a complete state context.
- **Constraints**: Enforces strict safety limits on the number of files and total directory size processed, configurable via `appsettings.json`, to prevent excessive resource usage during scanning.
- **Resilience**: Gracefully handles `UnauthorizedAccessException`, `PathTooLongException`, and `DirectoryNotFoundException` during the traversal, skipping problematic files while maintaining scan progress.

## Getting Started

1. Ensure the application is configured with the correct paths in `appsettings.json`.
2. Run the project using `dotnet run` or via your IDE.
3. Use the web interface to input the root path you wish to monitor.

## Configuration

You can adjust file scan limits in `appsettings.json` under the `FileScanSettings` section:

- `MaxFileCount`: The maximum number of files to process per scan (default: 100).
- `MaxTotalSizeInBytes`: The maximum total size (in bytes) to process per scan (default: 52428800).

## Limitations

- **Server-side only**: The application can only scan files located on the server where it is running.
- **Permission Access**: The application may fail to access system or protected directories depending on the user privileges it runs with.
- **File Locking**: Files exclusively locked by other processes may not be readable during the scan.

---

# Český překlad (Czech Translation)

FileX je aplikace ASP.NET Core navržená ke sledování změn v souborovém systému pořizováním a porovnáváním snímků (snapshotů) adresáře.

## Funkce

- **Generování snímků**: Prohledejte kořenový adresář a vytvořte snímek jeho aktuálního stavu.
- **Sledování změn**: Porovnejte aktuální stav s předchozími snímky a identifikujte:
    - **Nové**: Soubory přidané od posledního skenování.
    - **Upravené**: Soubory, které se změnily (na základě hashe).
    - **Smazané**: Soubory, které již v aktuálním skenu neexistují.
- **Persistence**: Ukládá snímky lokálně pro historické sledování.
- **Sledování adresářů**: Monitoruje strukturu podadresářů pro detekci jejich přidání a odstranění.

## Technické detaily

FileX používá iterativní průchod adresářovou strukturou pro skenování souborových systémů. Při generování snímku aplikace:

- **Hashing**: Vypočítá SHA256 hash pro každý soubor, aby byla zajištěna přesná detekce změn. Hashing probíhá asynchronně pro optimalizaci výkonu.
- **Zachycení dat**: Zaznamená relativní cestu k souboru, velikost v bajtech a časové razítko poslední změny (`LastModifiedUtc`) pro vytvoření úplného kontextu stavu.
- **Omezení**: Vynucuje přísné bezpečnostní limity na počet souborů a celkovou velikost adresáře, konfigurovatelné přes `appsettings.json`, aby se zabránilo nadměrnému využití zdrojů během skenování.
- **Odolnost**: Elegantně zvládá `UnauthorizedAccessException`, `PathTooLongException` a `DirectoryNotFoundException` během průchodu, přeskakuje problematické soubory a zároveň udržuje průběh skenování.

## Začínáme

1. Ujistěte se, že je aplikace nakonfigurována se správnými cestami v `appsettings.json`.
2. Spusťte projekt pomocí `dotnet run` nebo přes vaše IDE.
3. Použijte webové rozhraní k zadání kořenové cesty, kterou chcete sledovat.

## Konfigurace

Limity skenování souborů můžete upravit v `appsettings.json` v sekci `FileScanSettings`:

- `MaxFileCount`: Maximální počet souborů ke zpracování na jeden sken (výchozí: 100).
- `MaxTotalSizeInBytes`: Maximální celková velikost (v bajtech) ke zpracování na jeden sken (výchozí: 52428800).

## Omezení

- **Pouze na straně serveru**: Aplikace může skenovat pouze soubory umístěné na serveru, kde běží.
- **Přístupová oprávnění**: Aplikace nemusí mít přístup k systémovým nebo chráněným adresářům v závislosti na uživatelských oprávněních, se kterými běží.
- **Zamykání souborů**: Soubory výhradně uzamčené jinými procesy nemusí být během skenování čitelné.