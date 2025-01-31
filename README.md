# StorybrewImageEditor
 Edit images, generate noise, create sprite groups, and more soon for Storybrew storyboards!
 This branch is for .NET 8.0, the main branch is dedicated to .NET 4.8
## How to set up
 To set up StorybrewImageEditor, you must go into your project inside Storybrew   
 and click "Settings" -> "Referenced Assemblies" -> "Add Assembly File"    
 Then, navigate to where you download SBImageLib.dll and select it.    

 You will also need to copy SBImageLib.dll and paste it inside of   
 the Storybrew root directory (at least, that's what I had to do).   

 This was used to make all of my storyboards from June 2024 onward.

### How to Use SBImageLib
 Much of SBImageLib relies on a base function, GenerateSprite(). This can be called using the following notation:

 ```
 OsbSprite spr = layer.GenerateSprite(basePath, origin, new ImageEditParams()
 {
    //gaussian blur
    BlurStrength = 5, //int

    //monochrome
    R = 0.4f, //floats
    G = 0.4f,
    B = 0.4f,

    //invert
    Invert = true, //bool

    //cropping
    CropBounds = Rectangle(x, y, width, height) //rectangle from System.Drawing
 });

 ```

 Another cool portion of SBImageLib is the ability to generate noise, without needing to source it online. This can be done using the following
 ```
 OsbAnimation anim = layer.GenerateNoise(
                                    seed, //int
                                    count, //int
                                    delay, //float
                                    NoiseType.FullColor //noisetype enum
                                    );
 ```

 Both of these methods create sprites directly usable inside Storybrew, with the same method calls for movement and scaling.

 You can also create shapes using the following notation:
 ```
 ShapeInfo s = Shape.GenerateShape(new BasicGeneratorParams()
 {
    Size = new Rectangle (x, y, width, height), // rectangle from System.Drawing
    Points = new Vector2[] {new Vector2(x1, y1), new Vector2(x2, y2)}, // Vector2[]
    LineColor = Color4.White, //color4
    FillColor = Color4.White, //Color4
    CurveType = CurveType.None, //curvetype enum
    LineWidth = 5 //int
 })

 //now to reference the shape, you call s.Path
 OsbSprite spr = layer.CreateSprite(s.Path, origin);

 //there is also the ability to draw basic shapes

 //draw a rectangle
 ShapeInfo s2 = Shape.GenerateRectangle(new BasicGeneratorParams()
 {
    Size = new Rectangle (x, y, width, height), // rectangle from System.Drawing
    LineColor = Color4.White, //color4
    FillColor = Color4.White, //Color4
    LineWidth = 5 //int
 });

 //draw an ellipse
 ShapeInfo s3 = Shape.GenerateEllipse(new BasicGeneratorParams()
 {
    Size = new Rectangle (x, y, width, height), // rectangle from System.Drawing
    LineColor = Color4.White, //color4
    FillColor = Color4.White, //Color4
    LineWidth = 5 //int
 });
 ```
 Lastly, you can also find all sprites in a folder (in cases where you are mass-producing sprites and aren't keeping track of them) using the following notation:
 ```
 // get all sprites in a folder, preformatted
 string[] spritepaths = GetAllSpritesFromFolder(basefolder);

 //to use these paths, use the following:
 OsbSprite s = layer.CreateSprite(spritepaths[index], origin);
 ```

 ### Cool effects I've pulled off using SBImageLib
 1. Glitching Sprite

 Here's the exact code snippet for glitching a sprite, which I used in the showstopper animation in Ludicin's Echoes of Memoria storyboard:

 NOTE: I had sprites in the folder that represented the individual channels (R, G, B)
 ```
 //iterate through elements
 int Height = 8;

 //NOTE: 85*8 = 680, which is then scaled down to 340. 
 //This is important, as the letterbox extended 70px on each side, 
 //adding up to 480 px height
 for (int i = 0; i < 85; i++)
            {
                //get red and blue strips
                OsbSprite red = layer.GenerateSprite("sb/groups/ground-r.PNG", OsbOrigin.Centre, new Rectangle(200, 200 + i*Height, 2400, Height));
                OsbSprite blue = layer.GenerateSprite("sb/groups/ground-b.PNG", OsbOrigin.Centre, new Rectangle(200, 200 + i*Height, 2400, Height));
                 
                //synchronize the glitches so that the random values are equal on both sides (mirrored)
                SynchronizeGlitches(red, blue, i, 0.1);
                
                //get green sprite
                OsbSprite green = layer.GenerateSprite("sb/groups/ground-g.PNG", OsbOrigin.Centre, new Rectangle(200, 200 + i*Height, 2400, Height));
                SynchronizeAdditives(green, 1);
                green.MoveY(StartTime, StartTime, 320, 72 + Height*0.5*i);
                double t = StartTime, rand = Random(100, 200);
                float dx = Random(-2f, 2f);
                
                //loop the sprite to oscillate
                green.StartLoopGroup(StartTime, (int)((EndTime - StartTime) / (rand*2f)) + 2);
                    green.MoveX(0, 0, 320 - dx, 320 + dx);
                    green.MoveX(rand, rand, 320 + dx, 320 - dx);
                    green.MoveX(rand*2, rand*2, 320 - dx, 320 + dx);
                green.EndGroup();
                
            }


 ```
 In a separate function, SynchronizeGlitches(), I had the following:
 ```
 internal void SynchronizeGlitches(OsbSprite r, OsbSprite b, int index, double factor = 1)
        {
            //synchronize additives just gave each sprite additive.
            SynchronizeAdditives(r, 1); SynchronizeAdditives(b, 1);
            //set position
            r.MoveY(StartTime, StartTime, 320, 72 + Height*0.5*index);
            b.MoveY(StartTime, StartTime, 320, 72 + Height*0.5*index);
            
            
            double d, rand;
            double time;
            
            double t = StartTime;
            while (t < EndTime)
            {
                d = Random(10, 60)*factor;
                time = Random(100, 200);
                rand = Random(20, 60) * (Random(-20, 20) < 0 ? -1:1) * factor;
                //glitch just moved the sprites in a mirrored order
                Glitch(r, b, OsbEasing.OutCirc, t, factor != 1 ? t+(rand*0.5):t, d, d - rand);
                //Glitch(r, b, OsbEasing.OutCirc, t+time, t+time, d - rand, d);

                t += Random(time*1.5d, time*2d);
            }

        }
 ```
 So in order to mimic the glitch effect, you need to: 
 1. Have 3 base sprites for each color channel (R, G, B)
 2. Create thin strips of each strip
 3. Layer the images and give them additive
 4. Move the red and blue sprites around in order to give the illusion of glitching.

### Credits
 I did code compilation and making it work inside Storybrew.  
    
 The gaussian blur effect was first coded by mdymel and Gisburne2000   
 almost 7 years ago!! Link to that here: https://github.com/mdymel/superfastblur

 Also, I learned how to use Marshal.Copy as a side effect from reading   
 this article on Gaussian Noise https://epochabuse.com/gaussian-noise/

