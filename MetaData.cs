using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Xmp;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace JPEG
{
    public class MetaData
    {
        public int width { get; set; }                      // 이미지 너비
        public int height { get; set; }                     // 이미지 높이
        public string create_datetime { get; set; }         // 촬영일
        public string body_serial { get; set; }             // 몸체 제조번호

        /* XMP Data */
        /* DJI Path : drone-dji*/

        public float latitude { get; set; }                 // 촬영 위도 : GpsLatitude
        public float longitude { get; set; }                // 촬영 경도 : GpsLongitude

        public float absolute_altitude { get; set; }        // 드론 절대 고도 : AbsoluteAltitude
        public float relative_altitude { get; set; }        // 드론 상대 고도 : RelativeAltitude

        public float drone_pitch { get; set; }              // 드론 pitch  : FlightPitchDegree
        public float drone_roll { get; set; }               // 드론 roll : FlightRollDegree
        public float drone_yaw { get; set; }                 // 드론 yaw : FlightYawDegree
        public float gimbal_pitch { get; set; }             // 짐벌 pitch : GimbalPitchDegree
        public float gimbal_roll { get; set; }              // 짐벌 roll : GimbalRollDegree
        public float gimbal_yaw { get; set; }               // 짐벌 yaw : GimbalYawDegree

        public MetaData(string file_path)
        {
            try
            {
                var readers = new IJpegSegmentMetadataReader[] { new ExifReader(), new XmpReader() };
                var directories = JpegMetadataReader.ReadMetadata(file_path, readers);

                // JPEG 파일 정보
                ExifSubIfdDirectory sub_directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                SetExifData(sub_directory);

                // DJI XMP 정보 설정
                // 드론, 짐벌 정보 
                XmpDirectory xmp_directory = directories.OfType<XmpDirectory>().FirstOrDefault();
                SetXmpData(xmp_directory);
            }
            catch (JpegProcessingException e)
            {
                throw e;
            }
            catch (IOException e)
            {
                throw e;
            }
        }


        public void SetExifData(ExifSubIfdDirectory Ifd0)
        {
            if (Ifd0 == null) return;

            foreach (var tag in Ifd0.Tags)
            {
                if (tag.Name.Contains("Image Width"))
                {
                    width = int.Parse(Regex.Replace(tag.Description, @"\D", ""));
                }
                else if (tag.Name.Contains("Image Height"))
                {
                    height = int.Parse(Regex.Replace(tag.Description, @"\D", ""));
                }
                else if (tag.Name.Contains("Date/Time Original"))
                {
                    create_datetime = tag.Description;
                }
                else if (tag.Name.Contains("Body Serial Number"))
                {
                    body_serial = tag.Description;
                }
            }
        }

        public void SetXmpData(XmpDirectory xmp)
        {
            if (xmp == null) return;

            foreach (var property in xmp.XmpMeta.Properties)
            {
                if (property.Path == null) continue;

                if (property.Path.Contains("GpsLatitude"))
                {
                    latitude = float.Parse(property.Value);
                }
                else if (property.Path.Contains("GpsLongitude"))
                {
                    longitude = float.Parse(property.Value);
                }
                else if (property.Path.Contains("AbsoluteAltitude"))
                {
                    absolute_altitude = float.Parse(property.Value);
                }
                else if (property.Path.Contains("RelativeAltitude"))
                {
                    relative_altitude = float.Parse(property.Value);
                }
                else if (property.Path.Contains("FlightPitchDegree"))
                {
                    drone_pitch = float.Parse(property.Value);
                }
                else if (property.Path.Contains("FlightRollDegree"))
                {
                    drone_roll = float.Parse(property.Value);
                }
                else if (property.Path.Contains("FlightYawDegree"))
                {
                    drone_yaw = float.Parse(property.Value);
                }
                else if (property.Path.Contains("GimbalPitchDegree"))
                {
                    gimbal_pitch = float.Parse(property.Value);
                }
                else if (property.Path.Contains("GimbalRollDegree"))
                {
                    gimbal_roll = float.Parse(property.Value);
                }
                else if (property.Path.Contains("GimbalYawDegree"))
                {
                    gimbal_yaw = float.Parse(property.Value);
                }
            }
        }
    }
}
