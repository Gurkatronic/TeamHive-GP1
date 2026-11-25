# Free sample import notes

The pack uses metres. GLB coordinates are Y-up with front +Z; FBX uses the authored Y-up/-Z-forward conversion.
Use the exported root transform at scale 1 and preserve socket empties. Set texture filtering to nearest/point
where the importer does not preserve it. Runtime images are no larger than 512 pixels.

GLBs embed their textures. For FBXs, keep `Textures` beside `Exports`, retaining all nested directories.
The sample uses shared texture atlases from the full pack; unused atlas regions are not extra supplied models.
Glass and liquid materials use transparency and may need the receiving engine's transparency settings.
The Black Brine liquid is the included liquid look. The Tonic Water bottle does not imply a separate tonic liquid material.

Counters and stools have floor pivots. The wall and floor are 2m modules. Place a tumbler liquid insert at the
same root position as its glass. Bottle closures have sockets; the ingredient variants already attach the
correct closure. `HB_Drink_LowTide` assembles the glass, liquid and garnish; add the separate ice cube if wanted.

The sample contains no lights, Blender source, engine project or furnished showcase scene. Supply lighting in
your renderer. No runtime drink-mixing code or PSX screen-space shader is included.

The source library was validated in Blender 5.1.2. Customer extraction checks also import the sample FBXs
with automatic image search disabled to verify the supplied texture paths. Engine-specific integration is unverified.
