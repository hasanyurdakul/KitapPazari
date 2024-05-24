using KitapPazariModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitapPazariDataAccess.Seed
{
    public class ProductSeed : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasData(
                new Product
                {
                    Id = 1,
                    Title = "Fareler ve İnsanlar",
                    Author = "John Steinbeck",
                    Description = "Pulitzer ve Nobel Edebiyat Ödülü’nü kazanan John Steinbeck’in çağımızın toplumsal ve insani meselelerini ustalıkla resmettiği eserleri modern dünya edebiyatının başyapıtları arasında yer alır. Steinbeck romanlarında yalın ve keskin bir gerçeklik sunarken yine de her seferinde çarpıcı bir öykü ile çıkar okurunun karşısına. Tarihin bir kesitindeki dramı insani ayrıntıları kaçırmadan sergilerken, \"tozpembe olmayan gerçekçi bir umudun\" türküsünü dillendirir. Bu nedenle eserleri edebi değerleri kadar güncelliklerini de hiç yitirmemiştir.\r\n\r\nFareler ve İnsanlar, birbirine zıt karakterdeki iki mevsimlik tarım işçisinin, zeki George Milton ve onun güçlü kuvvetli ama akli dengesi bozuk yoldaşı Lennie Small’un öyküsünü anlatır. Küçük bir toprak satın alıp insanca bir hayat yaşamanın hayalini kuran bu ikilinin öyküsünde dostluk ve dayanışma duygusu önemli bir yer tutar. Steinbeck insanın insanla ilişkisini anlatmakla kalmaz insanın doğayla ve toplumla kurduğu ilişkileri de konu eder bu destansı romanında. Kitabın ismine ilham veren Robert Burns şiirindeki gibi; \"En iyi planları farelerin ve insanların / Sıkça ters gider... ",
                    ISBN = "9789755705859",
                    ListPrice = 62.90 ,
                    Price = 62.90,
                    Price50 = 62.90,
                    Price100 = 62.90,
                    CategoryId = 1,
                    ImageURL=""
                },
                new Product
                {
                    Id = 2,
                    Title = "Yüzüklerin Efendisi",
                    Author = " J. R. R. Tolkien",
                    Description = "Dünya ikiye bölünmüştür, denir Tolkien'ın yapıtı söz konusu olduğunda: Yüzüklerin Efendisi'ni okumuş olanlar ve okuyacak olanlar. 1997 ile birlikte, çok sayıda Türkiyeli okur da \"okumuş olanlar\" safına geçme fırsatı buldu. Kitabın Türkçe basımı Yüzüklerin Efendisi'ne duyulan ilginin evrenselliğini kanıtladı.\r\n\r\nYapıtın bu başarısını taçlandırmak için üç kısmı bir araya getiren bu özel, tek cilt edisyonu sunuyoruz. Hem hâlâ okumamış, \"okuyacak olanlar\" için, hem de bu güzel kitabın kütüphanenizde gelecek kuşaklara devrolacak kadar kalıcı olması için...\r\n\r\nYüzüklerin Efendisi yirminci yüzyılın en çok okunan yüz kitabı arasında en başta geliyor; bilimkurgu, fantazi, polisiye, best-seller ya da ana akım demeden, tüm edebiyat türleri arasında tartışmasız bir önderliğe sahip. Bir açıdan bakarsanız bir fantazi romanı, başka bir açıdan baktığınızda, insanlık durumu, sorumluluk, iktidar ve savaş üzerine bir roman. bir yolculuk, bir büyüme öyküsü; fedakarlık ve dostluk üzerine, hırs ve ihanet üzerine bir roman.",
                    ISBN = "9789753423472",
                    ListPrice = 549.90,
                    Price = 549.90,
                    Price50 = 549.90,
                    Price100 = 549.90,
                    CategoryId = 1,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "Şeker Portakalı",
                    Author = "Jose Mauro De Vasconcelos",
                    Description = "Yazarlıkta karar kılıncaya kadar, boks antrenörlüğünden ressam ve heykeltıraşlara modellik yapmaya, muz plantasyonlarında hamallıktan gece kulüplerinde garsonluğa kadar çeşitli işlerde çalışan Jose Mauro de Vasconcelos’un başyapıtı Şeker Portakalı, *günün birinde acıyı keşfeden küçük bir çocuğun öyküsü*dür. Çok yoksul bir ailenin oğlu olarak dünyaya gelen, dokuz yaşında yüzme öğrenirken bir gün yüzme şampiyonu olmanın hayalini kuran Vasconcelos’un çocukluğundan derin izler taşıyan Şeker Portakalı, yaşamın beklenmedik değişimleri karşısında büyük sarsıntılar yaşayan küçük Zeze’nin başından geçenleri anlatır. Vasconcelos, tam on iki günde yazdığı bu romanı *yirmi yıldan fazla bir zaman yüreğinde taşıdığını* söyler.\r\n\r\n",
                    ISBN = "9789750738609",
                    ListPrice = 101.85 ,
                    Price = 101.85,
                    Price50 = 101.85,
                    Price100 = 101.85,
                    CategoryId = 1,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 4,
                    Title = "Körlük",
                    Author = "Jose Saramago",
                    Description = "Usta yazarın belki de en etkileyici yapıtı olan, sinemaya da uyarlanmış Körlük, toplumsal yaşamın nasıl bir vahşete dönüşebileceğini müthiş bir incelikle gözler önüne sererken, insana dair son umut kırıntısını da bir kadının tek başına örgütlediği dayanışma ve direniş örneğiyle sergileyen unutulmaz eserler arasında yerini almıştır.",
                    ISBN = "9786254182228",
                    ListPrice = 155.75 ,
                    Price = 155.75,
                    Price50 = 155.75,
                    Price100 = 155.75,
                    CategoryId = 1,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 5,
                    Title = "Uğultulu Tepeler",
                    Author = "Emily Bronte",
                    Description = "Genç yaşta hayatını kaybeden Emily Jane Brontё’nin tek romanı olan Uğultulu Tepeler, okuyucu üzerinde büyük etki bırakıyor. Aynı zamanda bir şair olan yazar, şiirsel anlatım yeteneğini bu eserde de göstererek keyifli bir okuma sunuyor. İngiliz edebiyatının klasikleri arasında yer alan başarılı roman, oldukça farklı kurgusu ile okuyucuda merak uyandırıyor.\r\n\r\nAşk romanı olarak nitelendirilen bu eser, barındırdığı psikolojik ögelerle zenginleşerek beklentinin çok daha ötesine geçiyor. Victoria Dönemi’ni başarılı şekilde yansıtan kitabın, ayrıca yazarın hayatından da gerçek izler taşıdığı düşünülüyor. ",
                    ISBN = "9789750738913",
                    ListPrice = 81.50,
                    Price = 81.50,
                    Price50 = 81.50,
                    Price100 = 81.50,
                    CategoryId = 1,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 6,
                    Title = "İnci",
                    Author = "John Steinbeck",
                    Description = "Bir Meksika halk hikâyesinden esinlenmiş İnci, bir zamanlar İspanya Kralı'na büyük zenginlikler getiren bir koyda yaşayan fakir bir inci avcısının, Kino'nun ve ailesinin hikâyesini anlatır. Kino'nun çocuğunu kurtarmak umuduyla daldığı denizden çıkardığı eşi benzeri görülmemiş inci, yalnızca umut değil yıkım da getirecektir. İncinin özü insanların özüne; Kino'nun kulaklarında çınlayan ve kasabaya yayılan İncinin Türküsü, ailenin, kötülüğün, umudun ve düşmanlığın türküsüne karışacaktır.\r\nSteinbeck, Kino'nun derinliklerden söküp çıkardığı inci ile içinde yaşadığımız dünyaya ve insanın dramına ışık tutuyor.",
                    ISBN = "9789755705866",
                    ListPrice = 62.90,
                    Price = 62.90,
                    Price50 = 62.90,
                    Price100 = 62.90,
                    CategoryId = 1,
                    ImageURL = ""
                }, new Product
                {
                    Id = 7,
                    Title = "Puslu Kıtalar Atlası",
                    Author = "İhsan Oktay Anar",
                    Description = "Bir \"ilk kitap\", Türkçe edebiyatta yeni ve pırıltılı bir yazar... \"Yeniçeriler kapıyı zorlarken\" düşler üstüne düşüncelere dalan Uzun İhsan Efendi, kapı kırıldığında klasik ama hep yeni kalabilen sonuca ulaşmak üzeredir: \"Dünya bir düştür. Evet, dünya... Ah! Evet, dünya bir masaldır.\" Geçmiş üzerine, dünya hali üzerine, düşler ve \"puslu kıtalar\" üzerine bir roman. Hulki Aktunç`un önsözüyle...",
                    ISBN = "9789754704723",
                    ListPrice = 144.30,
                    Price = 144.30,
                    Price50 = 144.30,
                    Price100 = 144.30,
                    CategoryId = 1,
                    ImageURL = ""
                });
        }
    }
}
