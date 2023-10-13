shader_type canvas_item;

uniform sampler2D palette : hint_default_black; // Insert a palette
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear;
uniform int palette_size = 4;

// Color offset - changes threshold for color adjustments
uniform float offset = 0.5;

// Function to convert a pixel color into grayscale
vec4 to_grayscale(vec4 pixcol)
{
    float average = (pixcol.r + pixcol.g + pixcol.b) / 3.0;
    return vec4(average, average, average, 1);
}

// Colorizes the grayscale pixel
vec4 colorize(vec4 grayscale)
{
    // Color greater than (default) 0.75 in value
    // Set brightest color 1
    if(grayscale.r > offset * 1.5)
        return texture(palette, vec2(0.875, .0));
    
    // Color greater than (default) 0.50 in value
    // Set bright color 2
    if(grayscale.r > offset)
        return texture(palette, vec2(0.625, .0));
    
    // Color greater than (default) 0.25 in value
    // Set dark color 3
    if(grayscale.r > offset * 0.5)
        return texture(palette, vec2(0.375, .0));
    
    // Color greater than 0 in value
    // Set darkest color 4
    return texture(palette, vec2(0.125, .0));
}

void fragment(){ 
    // Get pixel color on screen 
    vec4 pixel_color =  vec4(texture(SCREEN_TEXTURE, SCREEN_UV)); 
    
    // Set the pixel color we are going to use
    COLOR = colorize(to_grayscale(pixel_color));
}