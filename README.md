# Film Tavsiye Uygulaması

Bu proje, C# Windows Forms ve MySQL veritabanı kullanılarak geliştirilmiş bir film tavsiye uygulamasıdır. Kullanıcılar, film adı, oyuncular, yönetmen, IMDB puanı ve türü gibi kriterlere göre filmleri filtreleyebilir, detayları görüntüleyebilir ve fragman izleyebilirler.

## Özellikler

- **Film Listeleme**: Kullanıcılar, ComboBox'lar aracılığıyla filtreleri belirleyerek filmleri listeler.
- **Detay Görüntüleme**: ListBox üzerinde seçilen film için poster ve plot bilgisi gösterilir.
- **Fragman İzleme**: Seçilen film için fragmanı YouTube üzerinde varsayılan tarayıcıda açma.
- **Dark Mode Desteği**: Kullanıcılar, arayüzü dark mode ve light mode arasında geçiş yapabilirler.

## Kullanım

1. **Başlangıç**: Uygulama başladığında, ilk olarak SplashForm görüntülenir ve ardından ana Form1 açılır.
2. **Film Filtreleme**: ComboBox'lar aracılığıyla filtreleri seçin ve **Listele** butonuna tıklayın.
3. **Film Detayları**: ListBox'tan bir film seçin, poster ve plot bilgisi görüntülenir.
4. **Fragman İzleme**: ListBox'tan bir film seçin ve **Fragman İzle** butonuna tıklayın.
5. **Dark Mode**: Arayüzü dark mode ve light mode arasında geçiş yapmak için **Dark Mode** butonuna tıklayın.

## Gereksinimler

- Visual Studio 2019 veya sonrası
- MySQL veritabanı (yerel veya uzak)

## Kurulum

1. Projenin GitHub repository'sini klonlayın.
2. MySQL veritabanını oluşturun ve `moviesdb` adında bir veritabanı oluşturun.
3. `film` tablosunu oluşturun ve gerekli sütunları ekleyin.
4. Proje dosyalarını Visual Studio'da açın.
5. `projedemo.sln` dosyasını çalıştırarak projeyi derleyin ve çalıştırın.

## Katkıda Bulunma

1. Bu repository'yi fork edin.
2. Yeni özellikler veya düzeltmeler ekleyin.
3. Pull request oluşturun.
