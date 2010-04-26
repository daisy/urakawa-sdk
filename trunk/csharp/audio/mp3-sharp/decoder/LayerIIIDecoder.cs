using Support;
/// <summary> 02/19/99 Java Conversion by E.B
/// -------------------------------------------------
/// layer3.h
/// *
/// Declarations for the Layer III decoder object
/// -------------------------------------------------
/// </summary>
/// <summary>*******************************************************************
/// date   programmers         comment                               *
/// *
/// 18/06/01  Michael Scheerer,  Fixed bugs which causes               *
/// negative indexes in method huffmann_decode and in method *
/// dequanisize_sample.                                      *
/// *
/// 16/07/01  Michael Scheerer, Catched a bug in method                *
/// huffmann_decode, which causes an outOfIndexException.    *
/// Cause : Indexnumber of 24 at SfBandIndex,                *
/// which has only a length of 22. I have simply and dirty   *
/// fixed the index to <= 22, because I'm not really be able *
/// to fix the bug. The Indexnumber is taken from the MP3    *
/// file and the origin Ma-Player with the same code works   *
/// well.                                                    *
/// *
/// *********************************************************************
/// *
/// ********************************************************************
/// </summary>
namespace javazoom.jl.decoder
{
    using System;
    /// <summary> Class Implementing Layer 3 Decoder.
    /// *
    /// @since 0.0
    /// </summary>

    sealed class LayerIIIDecoder : FrameDecoder
    {
        private void InitBlock()
        {
            rawout = new float[36];
            tsOutCopy = new float[18];
            is_ratio = new float[576];
            is_pos = new int[576];
            new_slen = new int[4];
            samples2 = new float[32];
            samples1 = new float[32];
        }
        public int[] scalefac_buffer;

        // MDM: removed, as this wasn't being used.
        //private float               CheckSumOut1d = 0.0f;
        private int CheckSumHuff = 0;
        private int[] is_1d;
        private float[][][] ro;
        private float[][][] lr;
        private float[] out_1d;
        private float[][] prevblck;
        private float[][] k;
        private int[] nonzero;
        private Bitstream stream;
        private Header header;
        private SynthesisFilter filter1, filter2;
        private Obuffer buffer;
        private int which_channels;
        private BitReserve br;
        private III_side_info_t si;

        private temporaire2[] III_scalefac_t;
        private temporaire2[] scalefac;
        // private III_scalefac_t 		scalefac;

        private int max_gr;
        private int frame_start;
        private int part2_start;
        private int channels;
        private int first_channel;
        private int last_channel;
        private int sfreq;


        /// <summary> Constructor.
        /// </summary>
        // REVIEW: these constructor arguments should be moved to the
        // decodeFrame() method, where possible, so that one
        public LayerIIIDecoder(Bitstream stream0, Header header0, SynthesisFilter filtera, SynthesisFilter filterb, Obuffer buffer0, int which_ch0)
        {
            InitBlock();
            huffcodetab.inithuff();
            is_1d = new int[SBLIMIT * SSLIMIT + 4];
            ro = new float[2][][];
            for (int i = 0; i < 2; i++)
            {
                ro[i] = new float[SBLIMIT][];
                for (int i2 = 0; i2 < SBLIMIT; i2++)
                {
                    ro[i][i2] = new float[SSLIMIT];
                }
            }
            lr = new float[2][][];
            for (int i3 = 0; i3 < 2; i3++)
            {
                lr[i3] = new float[SBLIMIT][];
                for (int i4 = 0; i4 < SBLIMIT; i4++)
                {
                    lr[i3][i4] = new float[SSLIMIT];
                }
            }
            out_1d = new float[SBLIMIT * SSLIMIT];
            prevblck = new float[2][];
            for (int i5 = 0; i5 < 2; i5++)
            {
                prevblck[i5] = new float[SBLIMIT * SSLIMIT];
            }
            k = new float[2][];
            for (int i6 = 0; i6 < 2; i6++)
            {
                k[i6] = new float[SBLIMIT * SSLIMIT];
            }
            nonzero = new int[2];

            //III_scalefact_t
            III_scalefac_t = new temporaire2[2];
            III_scalefac_t[0] = new temporaire2();
            III_scalefac_t[1] = new temporaire2();
            scalefac = III_scalefac_t;
            // L3TABLE INIT

            sfBandIndex = new SBI[9]; // SZD: MPEG2.5 +3 indices
            int[] l0 = new int[] { 0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 116, 140, 168, 200, 238, 284, 336, 396, 464, 522, 576 };
            int[] s0 = new int[] { 0, 4, 8, 12, 18, 24, 32, 42, 56, 74, 100, 132, 174, 192 };
            int[] l1 = new int[] { 0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 114, 136, 162, 194, 232, 278, 330, 394, 464, 540, 576 };
            int[] s1 = new int[] { 0, 4, 8, 12, 18, 26, 36, 48, 62, 80, 104, 136, 180, 192 };
            int[] l2 = new int[] { 0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 116, 140, 168, 200, 238, 284, 336, 396, 464, 522, 576 };
            int[] s2 = new int[] { 0, 4, 8, 12, 18, 26, 36, 48, 62, 80, 104, 134, 174, 192 };

            int[] l3 = new int[] { 0, 4, 8, 12, 16, 20, 24, 30, 36, 44, 52, 62, 74, 90, 110, 134, 162, 196, 238, 288, 342, 418, 576 };
            int[] s3 = new int[] { 0, 4, 8, 12, 16, 22, 30, 40, 52, 66, 84, 106, 136, 192 };
            int[] l4 = new int[] { 0, 4, 8, 12, 16, 20, 24, 30, 36, 42, 50, 60, 72, 88, 106, 128, 156, 190, 230, 276, 330, 384, 576 };
            int[] s4 = new int[] { 0, 4, 8, 12, 16, 22, 28, 38, 50, 64, 80, 100, 126, 192 };
            int[] l5 = new int[] { 0, 4, 8, 12, 16, 20, 24, 30, 36, 44, 54, 66, 82, 102, 126, 156, 194, 240, 296, 364, 448, 550, 576 };
            int[] s5 = new int[] { 0, 4, 8, 12, 16, 22, 30, 42, 58, 78, 104, 138, 180, 192 };
            // SZD: MPEG2.5
            int[] l6 = new int[] { 0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 116, 140, 168, 200, 238, 284, 336, 396, 464, 522, 576 };
            int[] s6 = new int[] { 0, 4, 8, 12, 18, 26, 36, 48, 62, 80, 104, 134, 174, 192 };
            int[] l7 = new int[] { 0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 116, 140, 168, 200, 238, 284, 336, 396, 464, 522, 576 };
            int[] s7 = new int[] { 0, 4, 8, 12, 18, 26, 36, 48, 62, 80, 104, 134, 174, 192 };
            int[] l8 = new int[] { 0, 12, 24, 36, 48, 60, 72, 88, 108, 132, 160, 192, 232, 280, 336, 400, 476, 566, 568, 570, 572, 574, 576 };
            int[] s8 = new int[] { 0, 8, 16, 24, 36, 52, 72, 96, 124, 160, 162, 164, 166, 192 };

            sfBandIndex[0] = new SBI(l0, s0);
            sfBandIndex[1] = new SBI(l1, s1);
            sfBandIndex[2] = new SBI(l2, s2);

            sfBandIndex[3] = new SBI(l3, s3);
            sfBandIndex[4] = new SBI(l4, s4);
            sfBandIndex[5] = new SBI(l5, s5);
            //SZD: MPEG2.5
            sfBandIndex[6] = new SBI(l6, s6);
            sfBandIndex[7] = new SBI(l7, s7);
            sfBandIndex[8] = new SBI(l8, s8);
            // END OF L3TABLE INIT

            if (reorder_table == null)
            {
                // SZD: generate LUT
                reorder_table = new int[9][];
                for (int i = 0; i < 9; i++)
                    reorder_table[i] = reorder(sfBandIndex[i].s);
            }

            // Sftable
            int[] ll0 = new int[] { 0, 6, 11, 16, 21 };
            int[] ss0 = new int[] { 0, 6, 12 };
            sftable = new Sftable(this, ll0, ss0);
            // END OF Sftable

            // scalefac_buffer
            scalefac_buffer = new int[54];
            // END OF scalefac_buffer

            stream = stream0;
            header = header0;
            filter1 = filtera;
            filter2 = filterb;
            buffer = buffer0;
            which_channels = which_ch0;

            frame_start = 0;
            channels = (header.mode() == Header.SINGLE_CHANNEL) ? 1 : 2;
            max_gr = (header.version() == Header.MPEG1) ? 2 : 1;

            sfreq = header.sample_frequency() + ((header.version() == Header.MPEG1) ? 3 : (header.version() == Header.MPEG25_LSF) ? 6 : 0); // SZD

            if (channels == 2)
            {
                switch (which_channels)
                {

                    case (int)OutputChannelsEnum.LEFT_CHANNEL:
                    case (int)OutputChannelsEnum.DOWNMIX_CHANNELS:
                        first_channel = last_channel = 0;
                        break;


                    case (int)OutputChannelsEnum.RIGHT_CHANNEL:
                        first_channel = last_channel = 1;
                        break;


                    case (int)OutputChannelsEnum.BOTH_CHANNELS:
                    default:
                        first_channel = 0;
                        last_channel = 1;
                        break;
                }
            }
            else
            {
                first_channel = last_channel = 0;
            }

            for (int ch = 0; ch < 2; ch++)
                for (int j = 0; j < 576; j++)
                    prevblck[ch][j] = 0.0f;

            nonzero[0] = nonzero[1] = 576;

            br = new BitReserve();
            si = new III_side_info_t();
        }

        /// <summary> Notify decoder that a seek is being made.
        /// </summary>
        public void seek_notify()
        {
            frame_start = 0;
            for (int ch = 0; ch < 2; ch++)
                for (int j = 0; j < 576; j++)
                    prevblck[ch][j] = 0.0f;
            br = new BitReserve();
        }

        public void decodeFrame()
        {
            decode();
        }

        /// <summary> Decode one frame, filling the buffer with the output samples.
        /// </summary>

        // subband samples are buffered and passed to the
        // SynthesisFilter in one go.
        //UPGRADE_NOTE: The initialization of  'samples1' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
        private float[] samples1;
        //UPGRADE_NOTE: The initialization of  'samples2' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
        private float[] samples2;

        public void decode()
        {
            int nSlots = header.slots();
            int flush_main;
            int gr, ch, ss, sb, sb18;
            int main_data_end;
            int bytes_to_discard;
            int i;

            get_side_info();

            for (i = 0; i < nSlots; i++)
                br.hputbuf(stream.get_bits(8));

            main_data_end = SupportClass.URShift(br.hsstell(), 3); // of previous frame

            if ((flush_main = (br.hsstell() & 7)) != 0)
            {
                br.hgetbits(8 - flush_main);
                main_data_end++;
            }

            bytes_to_discard = frame_start - main_data_end - si.main_data_begin;

            frame_start += nSlots;

            if (bytes_to_discard < 0)
                return;

            if (main_data_end > 4096)
            {
                frame_start -= 4096;
                br.rewindNbytes(4096);
            }

            for (; bytes_to_discard > 0; bytes_to_discard--)
                br.hgetbits(8);

            for (gr = 0; gr < max_gr; gr++)
            {

                for (ch = 0; ch < channels; ch++)
                {
                    part2_start = br.hsstell();

                    if (header.version() == Header.MPEG1)
                        get_scale_factors(ch, gr);
                    // MPEG-2 LSF, SZD: MPEG-2.5 LSF
                    else
                        get_LSF_scale_factors(ch, gr);

                    huffman_decode(ch, gr);
                    // System.out.println("CheckSum HuffMan = " + CheckSumHuff);
                    dequantize_sample(ro[ch], ch, gr);
                }

                stereo(gr);

                if ((which_channels == OutputChannels.DOWNMIX_CHANNELS) && (channels > 1))
                    do_downmix();

                for (ch = first_channel; ch <= last_channel; ch++)
                {

                    reorder(lr[ch], ch, gr);
                    antialias(ch, gr);
                    //for (int hb = 0;hb<576;hb++) CheckSumOut1d = CheckSumOut1d + out_1d[hb];
                    //System.out.println("CheckSumOut1d = "+CheckSumOut1d);

                    hybrid(ch, gr);

                    //for (int hb = 0;hb<576;hb++) CheckSumOut1d = CheckSumOut1d + out_1d[hb];
                    //System.out.println("CheckSumOut1d = "+CheckSumOut1d);

                    for (sb18 = 18; sb18 < 576; sb18 += 36)
                        // Frequency inversion
                        for (ss = 1; ss < SSLIMIT; ss += 2)
                            out_1d[sb18 + ss] = -out_1d[sb18 + ss];

                    if ((ch == 0) || (which_channels == OutputChannels.RIGHT_CHANNEL))
                    {
                        for (ss = 0; ss < SSLIMIT; ss++)
                        {
                            // Polyphase synthesis
                            sb = 0;
                            for (sb18 = 0; sb18 < 576; sb18 += 18)
                            {
                                samples1[sb] = out_1d[sb18 + ss];
                                //filter1.input_sample(out_1d[sb18+ss], sb);
                                sb++;
                            }
                            //buffer.appendSamples(0, samples1);
                            //Console.WriteLine("Adding samples right into output buffer");
                            filter1.input_samples(samples1);
                            filter1.calculate_pcm_samples(buffer);
                        }
                    }
                    else
                    {
                        for (ss = 0; ss < SSLIMIT; ss++)
                        {
                            // Polyphase synthesis
                            sb = 0;
                            for (sb18 = 0; sb18 < 576; sb18 += 18)
                            {
                                samples2[sb] = out_1d[sb18 + ss];
                                //filter2.input_sample(out_1d[sb18+ss], sb);
                                sb++;
                            }
                            filter2.input_samples(samples2);
                            filter2.calculate_pcm_samples(buffer);
                        }
                    }
                }
                // channels
            }
            // granule
            counter++;
            buffer.write_buffer(1);
        }

        /// <summary> Reads the side info from the stream, assuming the entire.
        /// frame has been read already.
        /// Mono   : 136 bits (= 17 bytes)
        /// Stereo : 256 bits (= 32 bytes)
        /// </summary>
        private bool get_side_info()
        {
            int ch, gr;
            if (header.version() == Header.MPEG1)
            {

                si.main_data_begin = stream.get_bits(9);
                if (channels == 1)
                    si.private_bits = stream.get_bits(5);
                else
                    si.private_bits = stream.get_bits(3);

                for (ch = 0; ch < channels; ch++)
                {
                    si.ch[ch].scfsi[0] = stream.get_bits(1);
                    si.ch[ch].scfsi[1] = stream.get_bits(1);
                    si.ch[ch].scfsi[2] = stream.get_bits(1);
                    si.ch[ch].scfsi[3] = stream.get_bits(1);
                }

                for (gr = 0; gr < 2; gr++)
                {
                    for (ch = 0; ch < channels; ch++)
                    {
                        si.ch[ch].gr[gr].part2_3_length = stream.get_bits(12);
                        si.ch[ch].gr[gr].big_values = stream.get_bits(9);
                        si.ch[ch].gr[gr].global_gain = stream.get_bits(8);
                        si.ch[ch].gr[gr].scalefac_compress = stream.get_bits(4);
                        si.ch[ch].gr[gr].window_switching_flag = stream.get_bits(1);
                        if ((si.ch[ch].gr[gr].window_switching_flag) != 0)
                        {
                            si.ch[ch].gr[gr].block_type = stream.get_bits(2);
                            si.ch[ch].gr[gr].mixed_block_flag = stream.get_bits(1);

                            si.ch[ch].gr[gr].table_select[0] = stream.get_bits(5);
                            si.ch[ch].gr[gr].table_select[1] = stream.get_bits(5);

                            si.ch[ch].gr[gr].subblock_gain[0] = stream.get_bits(3);
                            si.ch[ch].gr[gr].subblock_gain[1] = stream.get_bits(3);
                            si.ch[ch].gr[gr].subblock_gain[2] = stream.get_bits(3);

                            // Set region_count parameters since they are implicit in this case.

                            if (si.ch[ch].gr[gr].block_type == 0)
                            {
                                //	 Side info bad: block_type == 0 in split block
                                return false;
                            }
                            else if (si.ch[ch].gr[gr].block_type == 2 && si.ch[ch].gr[gr].mixed_block_flag == 0)
                            {
                                si.ch[ch].gr[gr].region0_count = 8;
                            }
                            else
                            {
                                si.ch[ch].gr[gr].region0_count = 7;
                            }
                            si.ch[ch].gr[gr].region1_count = 20 - si.ch[ch].gr[gr].region0_count;
                        }
                        else
                        {
                            si.ch[ch].gr[gr].table_select[0] = stream.get_bits(5);
                            si.ch[ch].gr[gr].table_select[1] = stream.get_bits(5);
                            si.ch[ch].gr[gr].table_select[2] = stream.get_bits(5);
                            si.ch[ch].gr[gr].region0_count = stream.get_bits(4);
                            si.ch[ch].gr[gr].region1_count = stream.get_bits(3);
                            si.ch[ch].gr[gr].block_type = 0;
                        }
                        si.ch[ch].gr[gr].preflag = stream.get_bits(1);
                        si.ch[ch].gr[gr].scalefac_scale = stream.get_bits(1);
                        si.ch[ch].gr[gr].count1table_select = stream.get_bits(1);
                    }
                }
            }
            else
            {
                // MPEG-2 LSF, SZD: MPEG-2.5 LSF

                si.main_data_begin = stream.get_bits(8);
                if (channels == 1)
                    si.private_bits = stream.get_bits(1);
                else
                    si.private_bits = stream.get_bits(2);

                for (ch = 0; ch < channels; ch++)
                {

                    si.ch[ch].gr[0].part2_3_length = stream.get_bits(12);
                    si.ch[ch].gr[0].big_values = stream.get_bits(9);
                    si.ch[ch].gr[0].global_gain = stream.get_bits(8);
                    si.ch[ch].gr[0].scalefac_compress = stream.get_bits(9);
                    si.ch[ch].gr[0].window_switching_flag = stream.get_bits(1);

                    if ((si.ch[ch].gr[0].window_switching_flag) != 0)
                    {

                        si.ch[ch].gr[0].block_type = stream.get_bits(2);
                        si.ch[ch].gr[0].mixed_block_flag = stream.get_bits(1);
                        si.ch[ch].gr[0].table_select[0] = stream.get_bits(5);
                        si.ch[ch].gr[0].table_select[1] = stream.get_bits(5);

                        si.ch[ch].gr[0].subblock_gain[0] = stream.get_bits(3);
                        si.ch[ch].gr[0].subblock_gain[1] = stream.get_bits(3);
                        si.ch[ch].gr[0].subblock_gain[2] = stream.get_bits(3);

                        // Set region_count parameters since they are implicit in this case.

                        if (si.ch[ch].gr[0].block_type == 0)
                        {
                            // Side info bad: block_type == 0 in split block
                            return false;
                        }
                        else if (si.ch[ch].gr[0].block_type == 2 && si.ch[ch].gr[0].mixed_block_flag == 0)
                        {
                            si.ch[ch].gr[0].region0_count = 8;
                        }
                        else
                        {
                            si.ch[ch].gr[0].region0_count = 7;
                            si.ch[ch].gr[0].region1_count = 20 - si.ch[ch].gr[0].region0_count;
                        }
                    }
                    else
                    {
                        si.ch[ch].gr[0].table_select[0] = stream.get_bits(5);
                        si.ch[ch].gr[0].table_select[1] = stream.get_bits(5);
                        si.ch[ch].gr[0].table_select[2] = stream.get_bits(5);
                        si.ch[ch].gr[0].region0_count = stream.get_bits(4);
                        si.ch[ch].gr[0].region1_count = stream.get_bits(3);
                        si.ch[ch].gr[0].block_type = 0;
                    }

                    si.ch[ch].gr[0].scalefac_scale = stream.get_bits(1);
                    si.ch[ch].gr[0].count1table_select = stream.get_bits(1);
                }
                // for(ch=0; ch<channels; ch++)
            }
            // if (header.version() == MPEG1)
            return true;
        }

        /// <summary>*
        /// </summary>
        private void get_scale_factors(int ch, int gr)
        {
            int sfb, window;
            gr_info_s gr_info = (si.ch[ch].gr[gr]);
            int scale_comp = gr_info.scalefac_compress;
            int length0 = slen[0][scale_comp];
            int length1 = slen[1][scale_comp];

            if ((gr_info.window_switching_flag != 0) && (gr_info.block_type == 2))
            {
                if ((gr_info.mixed_block_flag) != 0)
                {
                    // MIXED
                    for (sfb = 0; sfb < 8; sfb++)
                        scalefac[ch].l[sfb] = br.hgetbits(slen[0][gr_info.scalefac_compress]);
                    for (sfb = 3; sfb < 6; sfb++)
                        for (window = 0; window < 3; window++)
                            scalefac[ch].s[window][sfb] = br.hgetbits(slen[0][gr_info.scalefac_compress]);
                    for (sfb = 6; sfb < 12; sfb++)
                        for (window = 0; window < 3; window++)
                            scalefac[ch].s[window][sfb] = br.hgetbits(slen[1][gr_info.scalefac_compress]);
                    for (sfb = 12, window = 0; window < 3; window++)
                        scalefac[ch].s[window][sfb] = 0;
                }
                else
                {
                    // SHORT

                    scalefac[ch].s[0][0] = br.hgetbits(length0);
                    scalefac[ch].s[1][0] = br.hgetbits(length0);
                    scalefac[ch].s[2][0] = br.hgetbits(length0);
                    scalefac[ch].s[0][1] = br.hgetbits(length0);
                    scalefac[ch].s[1][1] = br.hgetbits(length0);
                    scalefac[ch].s[2][1] = br.hgetbits(length0);
                    scalefac[ch].s[0][2] = br.hgetbits(length0);
                    scalefac[ch].s[1][2] = br.hgetbits(length0);
                    scalefac[ch].s[2][2] = br.hgetbits(length0);
                    scalefac[ch].s[0][3] = br.hgetbits(length0);
                    scalefac[ch].s[1][3] = br.hgetbits(length0);
                    scalefac[ch].s[2][3] = br.hgetbits(length0);
                    scalefac[ch].s[0][4] = br.hgetbits(length0);
                    scalefac[ch].s[1][4] = br.hgetbits(length0);
                    scalefac[ch].s[2][4] = br.hgetbits(length0);
                    scalefac[ch].s[0][5] = br.hgetbits(length0);
                    scalefac[ch].s[1][5] = br.hgetbits(length0);
                    scalefac[ch].s[2][5] = br.hgetbits(length0);
                    scalefac[ch].s[0][6] = br.hgetbits(length1);
                    scalefac[ch].s[1][6] = br.hgetbits(length1);
                    scalefac[ch].s[2][6] = br.hgetbits(length1);
                    scalefac[ch].s[0][7] = br.hgetbits(length1);
                    scalefac[ch].s[1][7] = br.hgetbits(length1);
                    scalefac[ch].s[2][7] = br.hgetbits(length1);
                    scalefac[ch].s[0][8] = br.hgetbits(length1);
                    scalefac[ch].s[1][8] = br.hgetbits(length1);
                    scalefac[ch].s[2][8] = br.hgetbits(length1);
                    scalefac[ch].s[0][9] = br.hgetbits(length1);
                    scalefac[ch].s[1][9] = br.hgetbits(length1);
                    scalefac[ch].s[2][9] = br.hgetbits(length1);
                    scalefac[ch].s[0][10] = br.hgetbits(length1);
                    scalefac[ch].s[1][10] = br.hgetbits(length1);
                    scalefac[ch].s[2][10] = br.hgetbits(length1);
                    scalefac[ch].s[0][11] = br.hgetbits(length1);
                    scalefac[ch].s[1][11] = br.hgetbits(length1);
                    scalefac[ch].s[2][11] = br.hgetbits(length1);
                    scalefac[ch].s[0][12] = 0;
                    scalefac[ch].s[1][12] = 0;
                    scalefac[ch].s[2][12] = 0;
                }
                // SHORT
            }
            else
            {
                // LONG types 0,1,3

                if ((si.ch[ch].scfsi[0] == 0) || (gr == 0))
                {
                    scalefac[ch].l[0] = br.hgetbits(length0);
                    scalefac[ch].l[1] = br.hgetbits(length0);
                    scalefac[ch].l[2] = br.hgetbits(length0);
                    scalefac[ch].l[3] = br.hgetbits(length0);
                    scalefac[ch].l[4] = br.hgetbits(length0);
                    scalefac[ch].l[5] = br.hgetbits(length0);
                }
                if ((si.ch[ch].scfsi[1] == 0) || (gr == 0))
                {
                    scalefac[ch].l[6] = br.hgetbits(length0);
                    scalefac[ch].l[7] = br.hgetbits(length0);
                    scalefac[ch].l[8] = br.hgetbits(length0);
                    scalefac[ch].l[9] = br.hgetbits(length0);
                    scalefac[ch].l[10] = br.hgetbits(length0);
                }
                if ((si.ch[ch].scfsi[2] == 0) || (gr == 0))
                {
                    scalefac[ch].l[11] = br.hgetbits(length1);
                    scalefac[ch].l[12] = br.hgetbits(length1);
                    scalefac[ch].l[13] = br.hgetbits(length1);
                    scalefac[ch].l[14] = br.hgetbits(length1);
                    scalefac[ch].l[15] = br.hgetbits(length1);
                }
                if ((si.ch[ch].scfsi[3] == 0) || (gr == 0))
                {
                    scalefac[ch].l[16] = br.hgetbits(length1);
                    scalefac[ch].l[17] = br.hgetbits(length1);
                    scalefac[ch].l[18] = br.hgetbits(length1);
                    scalefac[ch].l[19] = br.hgetbits(length1);
                    scalefac[ch].l[20] = br.hgetbits(length1);
                }

                scalefac[ch].l[21] = 0;
                scalefac[ch].l[22] = 0;
            }
        }

        /// <summary>*
        /// </summary>
        // MDM: new_slen is fully initialized before use, no need
        // to reallocate array.
        //UPGRADE_NOTE: Final was removed from the declaration of 'new_slen '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
        //UPGRADE_NOTE: The initialization of  'new_slen' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
        private int[] new_slen;

        private void get_LSF_scale_data(int ch, int gr)
        {

            int scalefac_comp, int_scalefac_comp;
            int mode_ext = header.mode_extension();
            int m;
            int blocktypenumber;
            int blocknumber = 0;

            gr_info_s gr_info = (si.ch[ch].gr[gr]);

            scalefac_comp = gr_info.scalefac_compress;

            if (gr_info.block_type == 2)
            {
                if (gr_info.mixed_block_flag == 0)
                    blocktypenumber = 1;
                else if (gr_info.mixed_block_flag == 1)
                    blocktypenumber = 2;
                else
                    blocktypenumber = 0;
            }
            else
            {
                blocktypenumber = 0;
            }

            if (!(((mode_ext == 1) || (mode_ext == 3)) && (ch == 1)))
            {

                if (scalefac_comp < 400)
                {

                    new_slen[0] = (SupportClass.URShift(scalefac_comp, 4)) / 5;
                    new_slen[1] = (SupportClass.URShift(scalefac_comp, 4)) % 5;
                    new_slen[2] = SupportClass.URShift((scalefac_comp & 0xF), 2);
                    new_slen[3] = (scalefac_comp & 3);
                    si.ch[ch].gr[gr].preflag = 0;
                    blocknumber = 0;
                }
                else if (scalefac_comp < 500)
                {

                    new_slen[0] = (SupportClass.URShift((scalefac_comp - 400), 2)) / 5;
                    new_slen[1] = (SupportClass.URShift((scalefac_comp - 400), 2)) % 5;
                    new_slen[2] = (scalefac_comp - 400) & 3;
                    new_slen[3] = 0;
                    si.ch[ch].gr[gr].preflag = 0;
                    blocknumber = 1;
                }
                else if (scalefac_comp < 512)
                {

                    new_slen[0] = (scalefac_comp - 500) / 3;
                    new_slen[1] = (scalefac_comp - 500) % 3;
                    new_slen[2] = 0;
                    new_slen[3] = 0;
                    si.ch[ch].gr[gr].preflag = 1;
                    blocknumber = 2;
                }
            }

            if ((((mode_ext == 1) || (mode_ext == 3)) && (ch == 1)))
            {
                int_scalefac_comp = SupportClass.URShift(scalefac_comp, 1);

                if (int_scalefac_comp < 180)
                {
                    new_slen[0] = int_scalefac_comp / 36;
                    new_slen[1] = (int_scalefac_comp % 36) / 6;
                    new_slen[2] = (int_scalefac_comp % 36) % 6;
                    new_slen[3] = 0;
                    si.ch[ch].gr[gr].preflag = 0;
                    blocknumber = 3;
                }
                else if (int_scalefac_comp < 244)
                {
                    new_slen[0] = SupportClass.URShift(((int_scalefac_comp - 180) & 0x3F), 4);
                    new_slen[1] = SupportClass.URShift(((int_scalefac_comp - 180) & 0xF), 2);
                    new_slen[2] = (int_scalefac_comp - 180) & 3;
                    new_slen[3] = 0;
                    si.ch[ch].gr[gr].preflag = 0;
                    blocknumber = 4;
                }
                else if (int_scalefac_comp < 255)
                {
                    new_slen[0] = (int_scalefac_comp - 244) / 3;
                    new_slen[1] = (int_scalefac_comp - 244) % 3;
                    new_slen[2] = 0;
                    new_slen[3] = 0;
                    si.ch[ch].gr[gr].preflag = 0;
                    blocknumber = 5;
                }
            }

            for (int x = 0; x < 45; x++)
                // why 45, not 54?
                scalefac_buffer[x] = 0;

            m = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < nr_of_sfb_block[blocknumber][blocktypenumber][i]; j++)
                {
                    scalefac_buffer[m] = (new_slen[i] == 0) ? 0 : br.hgetbits(new_slen[i]);
                    m++;
                }
                // for (unint32 j ...
            }
            // for (uint32 i ...
        }

        /// <summary>*
        /// </summary>
        private void get_LSF_scale_factors(int ch, int gr)
        {
            int m = 0;
            int sfb, window;
            gr_info_s gr_info = (si.ch[ch].gr[gr]);

            get_LSF_scale_data(ch, gr);

            if ((gr_info.window_switching_flag != 0) && (gr_info.block_type == 2))
            {
                if (gr_info.mixed_block_flag != 0)
                {
                    // MIXED
                    for (sfb = 0; sfb < 8; sfb++)
                    {
                        scalefac[ch].l[sfb] = scalefac_buffer[m];
                        m++;
                    }
                    for (sfb = 3; sfb < 12; sfb++)
                    {
                        for (window = 0; window < 3; window++)
                        {
                            scalefac[ch].s[window][sfb] = scalefac_buffer[m];
                            m++;
                        }
                    }
                    for (window = 0; window < 3; window++)
                        scalefac[ch].s[window][12] = 0;
                }
                else
                {
                    // SHORT

                    for (sfb = 0; sfb < 12; sfb++)
                    {
                        for (window = 0; window < 3; window++)
                        {
                            scalefac[ch].s[window][sfb] = scalefac_buffer[m];
                            m++;
                        }
                    }

                    for (window = 0; window < 3; window++)
                        scalefac[ch].s[window][12] = 0;
                }
            }
            else
            {
                // LONG types 0,1,3

                for (sfb = 0; sfb < 21; sfb++)
                {
                    scalefac[ch].l[sfb] = scalefac_buffer[m];
                    m++;
                }
                scalefac[ch].l[21] = 0; // Jeff
                scalefac[ch].l[22] = 0;
            }
        }

        /// <summary>*
        /// </summary>
        public int[] x = new int[] { 0 };
        public int[] y = new int[] { 0 };
        public int[] v = new int[] { 0 };
        public int[] w = new int[] { 0 };
        private void huffman_decode(int ch, int gr)
        {
            x[0] = 0;
            y[0] = 0;
            v[0] = 0;
            w[0] = 0;

            int part2_3_end = part2_start + si.ch[ch].gr[gr].part2_3_length;
            int num_bits;
            int region1Start;
            int region2Start;
            int index;

            int buf, buf1;

            huffcodetab h;

            // Find region boundary for short block case

            if (((si.ch[ch].gr[gr].window_switching_flag) != 0) && (si.ch[ch].gr[gr].block_type == 2))
            {

                // Region2.
                //MS: Extrahandling for 8KHZ
                region1Start = (sfreq == 8) ? 72 : 36; // sfb[9/3]*3=36 or in case 8KHZ = 72
                region2Start = 576; // No Region2 for short block case
            }
            else
            {
                // Find region boundary for long block case

                buf = si.ch[ch].gr[gr].region0_count + 1;
                buf1 = buf + si.ch[ch].gr[gr].region1_count + 1;

                if (buf1 > sfBandIndex[sfreq].l.Length - 1)
                    buf1 = sfBandIndex[sfreq].l.Length - 1;

                region1Start = sfBandIndex[sfreq].l[buf];
                region2Start = sfBandIndex[sfreq].l[buf1]; /* MI */
            }

            index = 0;
            // Read bigvalues area
            for (int i = 0; i < (si.ch[ch].gr[gr].big_values << 1); i += 2)
            {
                if (i < region1Start)
                    h = huffcodetab.ht[si.ch[ch].gr[gr].table_select[0]];
                else if (i < region2Start)
                    h = huffcodetab.ht[si.ch[ch].gr[gr].table_select[1]];
                else
                    h = huffcodetab.ht[si.ch[ch].gr[gr].table_select[2]];

                huffcodetab.huffman_decoder(h, x, y, v, w, br);

                is_1d[index++] = x[0];
                is_1d[index++] = y[0];
                CheckSumHuff = CheckSumHuff + x[0] + y[0];
                // System.out.println("x = "+x[0]+" y = "+y[0]);
            }

            // Read count1 area
            h = huffcodetab.ht[si.ch[ch].gr[gr].count1table_select + 32];
            num_bits = br.hsstell();

            while ((num_bits < part2_3_end) && (index < 576))
            {

                huffcodetab.huffman_decoder(h, x, y, v, w, br);

                is_1d[index++] = v[0];
                is_1d[index++] = w[0];
                is_1d[index++] = x[0];
                is_1d[index++] = y[0];
                CheckSumHuff = CheckSumHuff + v[0] + w[0] + x[0] + y[0];
                // System.out.println("v = "+v[0]+" w = "+w[0]);
                // System.out.println("x = "+x[0]+" y = "+y[0]);
                num_bits = br.hsstell();
            }

            if (num_bits > part2_3_end)
            {
                br.rewindNbits(num_bits - part2_3_end);
                index -= 4;
            }

            num_bits = br.hsstell();

            // Dismiss stuffing bits
            if (num_bits < part2_3_end)
                br.hgetbits(part2_3_end - num_bits);

            // Zero out rest

            if (index < 576)
                nonzero[ch] = index;
            else
                nonzero[ch] = 576;

            if (index < 0)
                index = 0;

            // may not be necessary
            for (; index < 576; index++)
                is_1d[index] = 0;
        }

        /// <summary>*
        /// </summary>
        private void i_stereo_k_values(int is_pos, int io_type, int i)
        {
            if (is_pos == 0)
            {
                k[0][i] = 1.0f;
                k[1][i] = 1.0f;
            }
            else if ((is_pos & 1) != 0)
            {
                k[0][i] = io[io_type][SupportClass.URShift((is_pos + 1), 1)];
                k[1][i] = 1.0f;
            }
            else
            {
                k[0][i] = 1.0f;
                k[1][i] = io[io_type][SupportClass.URShift(is_pos, 1)];
            }
        }

        /// <summary>*
        /// </summary>
        private void dequantize_sample(float[][] xr, int ch, int gr)
        {
            gr_info_s gr_info = (si.ch[ch].gr[gr]);
            int cb = 0;
            int next_cb_boundary;
            int cb_begin = 0;
            int cb_width = 0;
            int index = 0, t_index, j;
            float g_gain;
            float[][] xr_1d = xr;

            // choose correct scalefactor band per block type, initalize boundary

            if ((gr_info.window_switching_flag != 0) && (gr_info.block_type == 2))
            {
                if (gr_info.mixed_block_flag != 0)
                    next_cb_boundary = sfBandIndex[sfreq].l[1];
                // LONG blocks: 0,1,3
                else
                {
                    cb_width = sfBandIndex[sfreq].s[1];
                    next_cb_boundary = (cb_width << 2) - cb_width;
                    cb_begin = 0;
                }
            }
            else
            {
                next_cb_boundary = sfBandIndex[sfreq].l[1]; // LONG blocks: 0,1,3
            }

            // Compute overall (global) scaling.

            g_gain = (float)System.Math.Pow(2.0, (0.25 * (gr_info.global_gain - 210.0)));

            for (j = 0; j < nonzero[ch]; j++)
            {
                try
                {
                    // Modif E.B 02/22/99
                    int reste = j % SSLIMIT;
                    int quotien = (int)((j - reste) / SSLIMIT);
                    if (is_1d[j] == 0)
                        xr_1d[quotien][reste] = 0.0f;
                    else
                    {
                        int abv = is_1d[j];
                        if (is_1d[j] > 0)
                            xr_1d[quotien][reste] = g_gain * t_43[abv];
                        else
                            xr_1d[quotien][reste] = -g_gain * t_43[-abv];
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            // apply formula per block type

            for (j = 0; j < nonzero[ch]; j++)
            {
                // Modif E.B 02/22/99
                int reste = j % SSLIMIT;
                int quotien = (int)((j - reste) / SSLIMIT);

                if (index == next_cb_boundary)
                {
                    /* Adjust critical band boundary */
                    if ((gr_info.window_switching_flag != 0) && (gr_info.block_type == 2))
                    {
                        if (gr_info.mixed_block_flag != 0)
                        {

                            if (index == sfBandIndex[sfreq].l[8])
                            {
                                next_cb_boundary = sfBandIndex[sfreq].s[4];
                                next_cb_boundary = (next_cb_boundary << 2) - next_cb_boundary;
                                cb = 3;
                                cb_width = sfBandIndex[sfreq].s[4] - sfBandIndex[sfreq].s[3];

                                cb_begin = sfBandIndex[sfreq].s[3];
                                cb_begin = (cb_begin << 2) - cb_begin;
                            }
                            else if (index < sfBandIndex[sfreq].l[8])
                            {

                                next_cb_boundary = sfBandIndex[sfreq].l[(++cb) + 1];
                            }
                            else
                            {

                                next_cb_boundary = sfBandIndex[sfreq].s[(++cb) + 1];
                                next_cb_boundary = (next_cb_boundary << 2) - next_cb_boundary;

                                cb_begin = sfBandIndex[sfreq].s[cb];
                                cb_width = sfBandIndex[sfreq].s[cb + 1] - cb_begin;
                                cb_begin = (cb_begin << 2) - cb_begin;
                            }
                        }
                        else
                        {

                            next_cb_boundary = sfBandIndex[sfreq].s[(++cb) + 1];
                            next_cb_boundary = (next_cb_boundary << 2) - next_cb_boundary;

                            cb_begin = sfBandIndex[sfreq].s[cb];
                            cb_width = sfBandIndex[sfreq].s[cb + 1] - cb_begin;
                            cb_begin = (cb_begin << 2) - cb_begin;
                        }
                    }
                    else
                    {
                        // long blocks

                        next_cb_boundary = sfBandIndex[sfreq].l[(++cb) + 1];
                    }
                }

                // Do long/short dependent scaling operations

                if ((gr_info.window_switching_flag != 0) && (((gr_info.block_type == 2) && (gr_info.mixed_block_flag == 0)) || ((gr_info.block_type == 2) && (gr_info.mixed_block_flag != 0) && (j >= 36))))
                {

                    t_index = (index - cb_begin) / cb_width;
                    /*            xr[sb][ss] *= pow(2.0, ((-2.0 * gr_info.subblock_gain[t_index])
                    -(0.5 * (1.0 + gr_info.scalefac_scale)
                    * scalefac[ch].s[t_index][cb]))); */
                    int idx = scalefac[ch].s[t_index][cb] << gr_info.scalefac_scale;
                    idx += (gr_info.subblock_gain[t_index] << 2);

                    xr_1d[quotien][reste] *= two_to_negative_half_pow[idx];
                }
                else
                {
                    // LONG block types 0,1,3 & 1st 2 subbands of switched blocks
                    /*				xr[sb][ss] *= pow(2.0, -0.5 * (1.0+gr_info.scalefac_scale)
                    * (scalefac[ch].l[cb]
                    + gr_info.preflag * pretab[cb])); */
                    int idx = scalefac[ch].l[cb];

                    if (gr_info.preflag != 0)
                        idx += pretab[cb];

                    idx = idx << gr_info.scalefac_scale;
                    xr_1d[quotien][reste] *= two_to_negative_half_pow[idx];
                }
                index++;
            }

            for (j = nonzero[ch]; j < 576; j++)
            {
                // Modif E.B 02/22/99
                int reste = j % SSLIMIT;
                int quotien = (int)((j - reste) / SSLIMIT);
                if (reste < 0)
                    reste = 0;
                if (quotien < 0)
                    quotien = 0;
                xr_1d[quotien][reste] = 0.0f;
            }

            return;
        }

        /// <summary>*
        /// </summary>
        private void reorder(float[][] xr, int ch, int gr)
        {
            gr_info_s gr_info = (si.ch[ch].gr[gr]);
            int freq, freq3;
            int index;
            int sfb, sfb_start, sfb_lines;
            int src_line, des_line;
            float[][] xr_1d = xr;

            if ((gr_info.window_switching_flag != 0) && (gr_info.block_type == 2))
            {

                for (index = 0; index < 576; index++)
                    out_1d[index] = 0.0f;

                if (gr_info.mixed_block_flag != 0)
                {
                    // NO REORDER FOR LOW 2 SUBBANDS
                    for (index = 0; index < 36; index++)
                    {
                        // Modif E.B 02/22/99
                        int reste = index % SSLIMIT;
                        int quotien = (int)((index - reste) / SSLIMIT);
                        out_1d[index] = xr_1d[quotien][reste];
                    }
                    // REORDERING FOR REST SWITCHED SHORT
                    for (sfb = 3, sfb_start = sfBandIndex[sfreq].s[3], sfb_lines = sfBandIndex[sfreq].s[4] - sfb_start; sfb < 13; sfb++, sfb_start = sfBandIndex[sfreq].s[sfb], sfb_lines = sfBandIndex[sfreq].s[sfb + 1] - sfb_start)
                    {
                        int sfb_start3 = (sfb_start << 2) - sfb_start;

                        for (freq = 0, freq3 = 0; freq < sfb_lines; freq++, freq3 += 3)
                        {

                            src_line = sfb_start3 + freq;
                            des_line = sfb_start3 + freq3;
                            // Modif E.B 02/22/99
                            int reste = src_line % SSLIMIT;
                            int quotien = (int)((src_line - reste) / SSLIMIT);

                            out_1d[des_line] = xr_1d[quotien][reste];
                            src_line += sfb_lines;
                            des_line++;

                            reste = src_line % SSLIMIT;
                            quotien = (int)((src_line - reste) / SSLIMIT);

                            out_1d[des_line] = xr_1d[quotien][reste];
                            src_line += sfb_lines;
                            des_line++;

                            reste = src_line % SSLIMIT;
                            quotien = (int)((src_line - reste) / SSLIMIT);

                            out_1d[des_line] = xr_1d[quotien][reste];
                        }
                    }
                }
                else
                {
                    // pure short
                    for (index = 0; index < 576; index++)
                    {
                        int j = reorder_table[sfreq][index];
                        int reste = j % SSLIMIT;
                        int quotien = (int)((j - reste) / SSLIMIT);
                        out_1d[index] = xr_1d[quotien][reste];
                    }
                }
            }
            else
            {
                // long blocks
                for (index = 0; index < 576; index++)
                {
                    // Modif E.B 02/22/99
                    int reste = index % SSLIMIT;
                    int quotien = (int)((index - reste) / SSLIMIT);
                    out_1d[index] = xr_1d[quotien][reste];
                }
            }
        }

        /// <summary>*
        /// </summary>

        //UPGRADE_NOTE: The initialization of  'is_pos' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
        public int[] is_pos;
        //UPGRADE_NOTE: The initialization of  'is_ratio' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
        public float[] is_ratio;

        private void stereo(int gr)
        {
            int sb, ss;

            if (channels == 1)
            {
                // mono , bypass xr[0][][] to lr[0][][]

                for (sb = 0; sb < SBLIMIT; sb++)
                    for (ss = 0; ss < SSLIMIT; ss += 3)
                    {
                        lr[0][sb][ss] = ro[0][sb][ss];
                        lr[0][sb][ss + 1] = ro[0][sb][ss + 1];
                        lr[0][sb][ss + 2] = ro[0][sb][ss + 2];
                    }
            }
            else
            {

                gr_info_s gr_info = (si.ch[0].gr[gr]);
                int mode_ext = header.mode_extension();
                int sfb;
                int i;
                int lines, temp, temp2;

                bool ms_stereo = ((header.mode() == Header.JOINT_STEREO) && ((mode_ext & 0x2) != 0));
                bool i_stereo = ((header.mode() == Header.JOINT_STEREO) && ((mode_ext & 0x1) != 0));
                bool lsf = ((header.version() == Header.MPEG2_LSF || header.version() == Header.MPEG25_LSF)); // SZD

                int io_type = (gr_info.scalefac_compress & 1);

                // initialization

                for (i = 0; i < 576; i++)
                {
                    is_pos[i] = 7;

                    is_ratio[i] = 0.0f;
                }

                if (i_stereo)
                {
                    if ((gr_info.window_switching_flag != 0) && (gr_info.block_type == 2))
                    {
                        if (gr_info.mixed_block_flag != 0)
                        {

                            int max_sfb = 0;

                            for (int j = 0; j < 3; j++)
                            {
                                int sfbcnt;
                                sfbcnt = 2;
                                for (sfb = 12; sfb >= 3; sfb--)
                                {
                                    i = sfBandIndex[sfreq].s[sfb];
                                    lines = sfBandIndex[sfreq].s[sfb + 1] - i;
                                    i = (i << 2) - i + (j + 1) * lines - 1;

                                    while (lines > 0)
                                    {
                                        if (ro[1][i / 18][i % 18] != 0.0f)
                                        {
                                            // MDM: in java, array access is very slow.
                                            // Is quicker to compute div and mod values.
                                            //if (ro[1][ss_div[i]][ss_mod[i]] != 0.0f) {
                                            sfbcnt = sfb;
                                            sfb = -10;
                                            lines = -10;
                                        }

                                        lines--;
                                        i--;
                                    } // while (lines > 0)
                                }
                                // for (sfb=12 ...
                                sfb = sfbcnt + 1;

                                if (sfb > max_sfb)
                                    max_sfb = sfb;

                                while (sfb < 12)
                                {
                                    temp = sfBandIndex[sfreq].s[sfb];
                                    sb = sfBandIndex[sfreq].s[sfb + 1] - temp;
                                    i = (temp << 2) - temp + j * sb;

                                    for (; sb > 0; sb--)
                                    {
                                        is_pos[i] = scalefac[1].s[j][sfb];
                                        if (is_pos[i] != 7)
                                            if (lsf)
                                                i_stereo_k_values(is_pos[i], io_type, i);
                                            else
                                                is_ratio[i] = TAN12[is_pos[i]];

                                        i++;
                                    }
                                    // for (; sb>0...
                                    sfb++;
                                } // while (sfb < 12)
                                sfb = sfBandIndex[sfreq].s[10];
                                sb = sfBandIndex[sfreq].s[11] - sfb;
                                sfb = (sfb << 2) - sfb + j * sb;
                                temp = sfBandIndex[sfreq].s[11];
                                sb = sfBandIndex[sfreq].s[12] - temp;
                                i = (temp << 2) - temp + j * sb;

                                for (; sb > 0; sb--)
                                {
                                    is_pos[i] = is_pos[sfb];

                                    if (lsf)
                                    {
                                        k[0][i] = k[0][sfb];
                                        k[1][i] = k[1][sfb];
                                    }
                                    else
                                    {
                                        is_ratio[i] = is_ratio[sfb];
                                    }
                                    i++;
                                }
                                // for (; sb > 0 ...
                            }
                            if (max_sfb <= 3)
                            {
                                i = 2;
                                ss = 17;
                                sb = -1;
                                while (i >= 0)
                                {
                                    if (ro[1][i][ss] != 0.0f)
                                    {
                                        sb = (i << 4) + (i << 1) + ss;
                                        i = -1;
                                    }
                                    else
                                    {
                                        ss--;
                                        if (ss < 0)
                                        {
                                            i--;
                                            ss = 17;
                                        }
                                    }
                                    // if (ro ...
                                } // while (i>=0)
                                i = 0;
                                while (sfBandIndex[sfreq].l[i] <= sb)
                                    i++;
                                sfb = i;
                                i = sfBandIndex[sfreq].l[i];
                                for (; sfb < 8; sfb++)
                                {
                                    sb = sfBandIndex[sfreq].l[sfb + 1] - sfBandIndex[sfreq].l[sfb];
                                    for (; sb > 0; sb--)
                                    {
                                        is_pos[i] = scalefac[1].l[sfb];
                                        if (is_pos[i] != 7)
                                            if (lsf)
                                                i_stereo_k_values(is_pos[i], io_type, i);
                                            else
                                                is_ratio[i] = TAN12[is_pos[i]];
                                        i++;
                                    }
                                    // for (; sb>0 ...
                                }
                                // for (; sfb<8 ...
                            }
                            // for (j=0 ...
                        }
                        else
                        {
                            // if (gr_info.mixed_block_flag)
                            for (int j = 0; j < 3; j++)
                            {
                                int sfbcnt;
                                sfbcnt = -1;
                                for (sfb = 12; sfb >= 0; sfb--)
                                {
                                    temp = sfBandIndex[sfreq].s[sfb];
                                    lines = sfBandIndex[sfreq].s[sfb + 1] - temp;
                                    i = (temp << 2) - temp + (j + 1) * lines - 1;

                                    while (lines > 0)
                                    {
                                        if (ro[1][i / 18][i % 18] != 0.0f)
                                        {
                                            // MDM: in java, array access is very slow.
                                            // Is quicker to compute div and mod values.
                                            //if (ro[1][ss_div[i]][ss_mod[i]] != 0.0f) {
                                            sfbcnt = sfb;
                                            sfb = -10;
                                            lines = -10;
                                        }
                                        lines--;
                                        i--;
                                    } // while (lines > 0) */
                                }
                                // for (sfb=12 ...
                                sfb = sfbcnt + 1;
                                while (sfb < 12)
                                {
                                    temp = sfBandIndex[sfreq].s[sfb];
                                    sb = sfBandIndex[sfreq].s[sfb + 1] - temp;
                                    i = (temp << 2) - temp + j * sb;
                                    for (; sb > 0; sb--)
                                    {
                                        is_pos[i] = scalefac[1].s[j][sfb];
                                        if (is_pos[i] != 7)
                                            if (lsf)
                                                i_stereo_k_values(is_pos[i], io_type, i);
                                            else
                                                is_ratio[i] = TAN12[is_pos[i]];
                                        i++;
                                    }
                                    // for (; sb>0 ...
                                    sfb++;
                                } // while (sfb<12)

                                temp = sfBandIndex[sfreq].s[10];
                                temp2 = sfBandIndex[sfreq].s[11];
                                sb = temp2 - temp;
                                sfb = (temp << 2) - temp + j * sb;
                                sb = sfBandIndex[sfreq].s[12] - temp2;
                                i = (temp2 << 2) - temp2 + j * sb;

                                for (; sb > 0; sb--)
                                {
                                    is_pos[i] = is_pos[sfb];

                                    if (lsf)
                                    {
                                        k[0][i] = k[0][sfb];
                                        k[1][i] = k[1][sfb];
                                    }
                                    else
                                    {
                                        is_ratio[i] = is_ratio[sfb];
                                    }
                                    i++;
                                }
                                // for (; sb>0 ...
                            }
                            // for (sfb=12
                        }
                        // for (j=0 ...
                    }
                    else
                    {
                        // if (gr_info.window_switching_flag ...
                        i = 31;
                        ss = 17;
                        sb = 0;
                        while (i >= 0)
                        {
                            if (ro[1][i][ss] != 0.0f)
                            {
                                sb = (i << 4) + (i << 1) + ss;
                                i = -1;
                            }
                            else
                            {
                                ss--;
                                if (ss < 0)
                                {
                                    i--;
                                    ss = 17;
                                }
                            }
                        }
                        i = 0;
                        while (sfBandIndex[sfreq].l[i] <= sb)
                            i++;

                        sfb = i;
                        i = sfBandIndex[sfreq].l[i];
                        for (; sfb < 21; sfb++)
                        {
                            sb = sfBandIndex[sfreq].l[sfb + 1] - sfBandIndex[sfreq].l[sfb];
                            for (; sb > 0; sb--)
                            {
                                is_pos[i] = scalefac[1].l[sfb];
                                if (is_pos[i] != 7)
                                    if (lsf)
                                        i_stereo_k_values(is_pos[i], io_type, i);
                                    else
                                        is_ratio[i] = TAN12[is_pos[i]];
                                i++;
                            }
                        }
                        sfb = sfBandIndex[sfreq].l[20];
                        for (sb = 576 - sfBandIndex[sfreq].l[21]; (sb > 0) && (i < 576); sb--)
                        {
                            is_pos[i] = is_pos[sfb]; // error here : i >=576

                            if (lsf)
                            {
                                k[0][i] = k[0][sfb];
                                k[1][i] = k[1][sfb];
                            }
                            else
                            {
                                is_ratio[i] = is_ratio[sfb];
                            }
                            i++;
                        }
                        // if (gr_info.mixed_block_flag)
                    }
                    // if (gr_info.window_switching_flag ...
                }
                // if (i_stereo)

                i = 0;
                for (sb = 0; sb < SBLIMIT; sb++)
                    for (ss = 0; ss < SSLIMIT; ss++)
                    {
                        if (is_pos[i] == 7)
                        {
                            if (ms_stereo)
                            {
                                lr[0][sb][ss] = (ro[0][sb][ss] + ro[1][sb][ss]) * 0.707106781f;
                                lr[1][sb][ss] = (ro[0][sb][ss] - ro[1][sb][ss]) * 0.707106781f;
                            }
                            else
                            {
                                lr[0][sb][ss] = ro[0][sb][ss];
                                lr[1][sb][ss] = ro[1][sb][ss];
                            }
                        }
                        else if (i_stereo)
                        {

                            if (lsf)
                            {
                                lr[0][sb][ss] = ro[0][sb][ss] * k[0][i];
                                lr[1][sb][ss] = ro[0][sb][ss] * k[1][i];
                            }
                            else
                            {
                                lr[1][sb][ss] = ro[0][sb][ss] / (float)(1 + is_ratio[i]);
                                lr[0][sb][ss] = lr[1][sb][ss] * is_ratio[i];
                            }
                        }
                        /*				else {
                        System.out.println("Error in stereo processing\n");
                        } */
                        i++;
                    }
            }
            // channels == 2
        }

        /// <summary>*
        /// </summary>
        private void antialias(int ch, int gr)
        {
            int sb18, ss, sb18lim;
            gr_info_s gr_info = (si.ch[ch].gr[gr]);
            // 31 alias-reduction operations between each pair of sub-bands
            // with 8 butterflies between each pair

            if ((gr_info.window_switching_flag != 0) && (gr_info.block_type == 2) && !(gr_info.mixed_block_flag != 0))
                return;

            if ((gr_info.window_switching_flag != 0) && (gr_info.mixed_block_flag != 0) && (gr_info.block_type == 2))
            {
                sb18lim = 18;
            }
            else
            {
                sb18lim = 558;
            }

            for (sb18 = 0; sb18 < sb18lim; sb18 += 18)
            {
                for (ss = 0; ss < 8; ss++)
                {
                    int src_idx1 = sb18 + 17 - ss;
                    int src_idx2 = sb18 + 18 + ss;
                    float bu = out_1d[src_idx1];
                    float bd = out_1d[src_idx2];
                    out_1d[src_idx1] = (bu * cs[ss]) - (bd * ca[ss]);
                    out_1d[src_idx2] = (bd * cs[ss]) + (bu * ca[ss]);
                }
            }
        }

        /// <summary>*
        /// </summary>

        // MDM: tsOutCopy and rawout do not need initializing, so the arrays
        // can be reused.
        //UPGRADE_NOTE: The initialization of  'tsOutCopy' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
        public float[] tsOutCopy;
        //UPGRADE_NOTE: The initialization of  'rawout' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
        public float[] rawout;

        private void hybrid(int ch, int gr)
        {
            int bt;
            int sb18;
            gr_info_s gr_info = (si.ch[ch].gr[gr]);
            float[] tsOut;

            float[][] prvblk;

            for (sb18 = 0; sb18 < 576; sb18 += 18)
            {
                bt = ((gr_info.window_switching_flag != 0) && (gr_info.mixed_block_flag != 0) && (sb18 < 36)) ? 0 : gr_info.block_type;

                tsOut = out_1d;
                // Modif E.B 02/22/99
                for (int cc = 0; cc < 18; cc++)
                    tsOutCopy[cc] = tsOut[cc + sb18];

                inv_mdct(tsOutCopy, rawout, bt);


                for (int cc = 0; cc < 18; cc++)
                    tsOut[cc + sb18] = tsOutCopy[cc];
                // Fin Modif

                // overlap addition
                prvblk = prevblck;

                tsOut[0 + sb18] = rawout[0] + prvblk[ch][sb18 + 0];
                prvblk[ch][sb18 + 0] = rawout[18];
                tsOut[1 + sb18] = rawout[1] + prvblk[ch][sb18 + 1];
                prvblk[ch][sb18 + 1] = rawout[19];
                tsOut[2 + sb18] = rawout[2] + prvblk[ch][sb18 + 2];
                prvblk[ch][sb18 + 2] = rawout[20];
                tsOut[3 + sb18] = rawout[3] + prvblk[ch][sb18 + 3];
                prvblk[ch][sb18 + 3] = rawout[21];
                tsOut[4 + sb18] = rawout[4] + prvblk[ch][sb18 + 4];
                prvblk[ch][sb18 + 4] = rawout[22];
                tsOut[5 + sb18] = rawout[5] + prvblk[ch][sb18 + 5];
                prvblk[ch][sb18 + 5] = rawout[23];
                tsOut[6 + sb18] = rawout[6] + prvblk[ch][sb18 + 6];
                prvblk[ch][sb18 + 6] = rawout[24];
                tsOut[7 + sb18] = rawout[7] + prvblk[ch][sb18 + 7];
                prvblk[ch][sb18 + 7] = rawout[25];
                tsOut[8 + sb18] = rawout[8] + prvblk[ch][sb18 + 8];
                prvblk[ch][sb18 + 8] = rawout[26];
                tsOut[9 + sb18] = rawout[9] + prvblk[ch][sb18 + 9];
                prvblk[ch][sb18 + 9] = rawout[27];
                tsOut[10 + sb18] = rawout[10] + prvblk[ch][sb18 + 10];
                prvblk[ch][sb18 + 10] = rawout[28];
                tsOut[11 + sb18] = rawout[11] + prvblk[ch][sb18 + 11];
                prvblk[ch][sb18 + 11] = rawout[29];
                tsOut[12 + sb18] = rawout[12] + prvblk[ch][sb18 + 12];
                prvblk[ch][sb18 + 12] = rawout[30];
                tsOut[13 + sb18] = rawout[13] + prvblk[ch][sb18 + 13];
                prvblk[ch][sb18 + 13] = rawout[31];
                tsOut[14 + sb18] = rawout[14] + prvblk[ch][sb18 + 14];
                prvblk[ch][sb18 + 14] = rawout[32];
                tsOut[15 + sb18] = rawout[15] + prvblk[ch][sb18 + 15];
                prvblk[ch][sb18 + 15] = rawout[33];
                tsOut[16 + sb18] = rawout[16] + prvblk[ch][sb18 + 16];
                prvblk[ch][sb18 + 16] = rawout[34];
                tsOut[17 + sb18] = rawout[17] + prvblk[ch][sb18 + 17];
                prvblk[ch][sb18 + 17] = rawout[35];
            }
        }

        /// <summary>*
        /// </summary>
        private void do_downmix()
        {
            for (int sb = 0; sb < SSLIMIT; sb++)
            {
                for (int ss = 0; ss < SSLIMIT; ss += 3)
                {
                    lr[0][sb][ss] = (lr[0][sb][ss] + lr[1][sb][ss]) * 0.5f;
                    lr[0][sb][ss + 1] = (lr[0][sb][ss + 1] + lr[1][sb][ss + 1]) * 0.5f;
                    lr[0][sb][ss + 2] = (lr[0][sb][ss + 2] + lr[1][sb][ss + 2]) * 0.5f;
                }
            }
        }

        /// <summary> Fast INV_MDCT.
        /// </summary>

        public void inv_mdct(float[] in_Renamed, float[] out_Renamed, int block_type)
        {
            float[] win_bt;
            int i;
            float tmpf_0, tmpf_1, tmpf_2, tmpf_3, tmpf_4, tmpf_5, tmpf_6, tmpf_7, tmpf_8, tmpf_9;
            float tmpf_10, tmpf_11, tmpf_12, tmpf_13, tmpf_14, tmpf_15, tmpf_16, tmpf_17;

            tmpf_0 = tmpf_1 = tmpf_2 = tmpf_3 = tmpf_4 = tmpf_5 = tmpf_6 = tmpf_7 = tmpf_8 = tmpf_9 = tmpf_10 = tmpf_11 = tmpf_12 = tmpf_13 = tmpf_14 = tmpf_15 = tmpf_16 = tmpf_17 = 0.0f;

            if (block_type == 2)
            {
                for (int looper = 0; looper < 36; looper++)
                    out_Renamed[looper] = 0.0f;

                int six_i = 0;

                for (i = 0; i < 3; i++)
                {
                    // 12 point IMDCT
                    // Begin 12 point IDCT
                    // Input aliasing for 12 pt IDCT
                    in_Renamed[15 + i] += in_Renamed[12 + i]; in_Renamed[12 + i] += in_Renamed[9 + i]; in_Renamed[9 + i] += in_Renamed[6 + i];
                    in_Renamed[6 + i] += in_Renamed[3 + i]; in_Renamed[3 + i] += in_Renamed[0 + i];

                    // Input aliasing on odd indices (for 6 point IDCT)
                    in_Renamed[15 + i] += in_Renamed[9 + i]; in_Renamed[9 + i] += in_Renamed[3 + i];

                    // 3 point IDCT on even indices
                    float pp1, pp2, sum;
                    pp2 = in_Renamed[12 + i] * 0.500000000f;
                    pp1 = in_Renamed[6 + i] * 0.866025403f;
                    sum = in_Renamed[0 + i] + pp2;
                    tmpf_1 = in_Renamed[0 + i] - in_Renamed[12 + i];
                    tmpf_0 = sum + pp1;
                    tmpf_2 = sum - pp1;

                    // End 3 point IDCT on even indices
                    // 3 point IDCT on odd indices (for 6 point IDCT)
                    pp2 = in_Renamed[15 + i] * 0.500000000f;
                    pp1 = in_Renamed[9 + i] * 0.866025403f;
                    sum = in_Renamed[3 + i] + pp2;
                    tmpf_4 = in_Renamed[3 + i] - in_Renamed[15 + i];
                    tmpf_5 = sum + pp1;
                    tmpf_3 = sum - pp1;
                    // End 3 point IDCT on odd indices
                    // Twiddle factors on odd indices (for 6 point IDCT)

                    tmpf_3 *= 1.931851653f;
                    tmpf_4 *= 0.707106781f;
                    tmpf_5 *= 0.517638090f;

                    // Output butterflies on 2 3 point IDCT's (for 6 point IDCT)
                    float save = tmpf_0;
                    tmpf_0 += tmpf_5;
                    tmpf_5 = save - tmpf_5;
                    save = tmpf_1;
                    tmpf_1 += tmpf_4;
                    tmpf_4 = save - tmpf_4;
                    save = tmpf_2;
                    tmpf_2 += tmpf_3;
                    tmpf_3 = save - tmpf_3;

                    // End 6 point IDCT
                    // Twiddle factors on indices (for 12 point IDCT)

                    tmpf_0 *= 0.504314480f;
                    tmpf_1 *= 0.541196100f;
                    tmpf_2 *= 0.630236207f;
                    tmpf_3 *= 0.821339815f;
                    tmpf_4 *= 1.306562965f;
                    tmpf_5 *= 3.830648788f;

                    // End 12 point IDCT

                    // Shift to 12 point modified IDCT, multiply by window type 2
                    tmpf_8 = -tmpf_0 * 0.793353340f;
                    tmpf_9 = -tmpf_0 * 0.608761429f;
                    tmpf_7 = -tmpf_1 * 0.923879532f;
                    tmpf_10 = -tmpf_1 * 0.382683432f;
                    tmpf_6 = -tmpf_2 * 0.991444861f;
                    tmpf_11 = -tmpf_2 * 0.130526192f;

                    tmpf_0 = tmpf_3;
                    tmpf_1 = tmpf_4 * 0.382683432f;
                    tmpf_2 = tmpf_5 * 0.608761429f;

                    tmpf_3 = -tmpf_5 * 0.793353340f;
                    tmpf_4 = -tmpf_4 * 0.923879532f;
                    tmpf_5 = -tmpf_0 * 0.991444861f;

                    tmpf_0 *= 0.130526192f;

                    out_Renamed[six_i + 6] += tmpf_0;
                    out_Renamed[six_i + 7] += tmpf_1;
                    out_Renamed[six_i + 8] += tmpf_2;
                    out_Renamed[six_i + 9] += tmpf_3;
                    out_Renamed[six_i + 10] += tmpf_4;
                    out_Renamed[six_i + 11] += tmpf_5;
                    out_Renamed[six_i + 12] += tmpf_6;
                    out_Renamed[six_i + 13] += tmpf_7;
                    out_Renamed[six_i + 14] += tmpf_8;
                    out_Renamed[six_i + 15] += tmpf_9;
                    out_Renamed[six_i + 16] += tmpf_10;
                    out_Renamed[six_i + 17] += tmpf_11;

                    six_i += 6;
                }
            }
            else
            {
                // 36 point IDCT
                // input aliasing for 36 point IDCT
                in_Renamed[17] += in_Renamed[16]; in_Renamed[16] += in_Renamed[15]; in_Renamed[15] += in_Renamed[14]; in_Renamed[14] += in_Renamed[13];
                in_Renamed[13] += in_Renamed[12]; in_Renamed[12] += in_Renamed[11]; in_Renamed[11] += in_Renamed[10]; in_Renamed[10] += in_Renamed[9];
                in_Renamed[9] += in_Renamed[8]; in_Renamed[8] += in_Renamed[7]; in_Renamed[7] += in_Renamed[6]; in_Renamed[6] += in_Renamed[5];
                in_Renamed[5] += in_Renamed[4]; in_Renamed[4] += in_Renamed[3]; in_Renamed[3] += in_Renamed[2]; in_Renamed[2] += in_Renamed[1];
                in_Renamed[1] += in_Renamed[0];

                // 18 point IDCT for odd indices
                // input aliasing for 18 point IDCT
                in_Renamed[17] += in_Renamed[15]; in_Renamed[15] += in_Renamed[13]; in_Renamed[13] += in_Renamed[11]; in_Renamed[11] += in_Renamed[9];
                in_Renamed[9] += in_Renamed[7]; in_Renamed[7] += in_Renamed[5]; in_Renamed[5] += in_Renamed[3]; in_Renamed[3] += in_Renamed[1];

                float tmp0, tmp1, tmp2, tmp3, tmp4, tmp0_, tmp1_, tmp2_, tmp3_;
                float tmp0o, tmp1o, tmp2o, tmp3o, tmp4o, tmp0_o, tmp1_o, tmp2_o, tmp3_o;

                // Fast 9 Point Inverse Discrete Cosine Transform
                //
                // By  Francois-Raymond Boyer
                //         mailto:boyerf@iro.umontreal.ca
                //         http://www.iro.umontreal.ca/~boyerf
                //
                // The code has been optimized for Intel processors
                //  (takes a lot of time to convert float to and from iternal FPU representation)
                //
                // It is a simple "factorization" of the IDCT matrix.

                // 9 point IDCT on even indices

                // 5 points on odd indices (not realy an IDCT)
                float i00 = in_Renamed[0] + in_Renamed[0];
                float iip12 = i00 + in_Renamed[12];

                tmp0 = iip12 + in_Renamed[4] * 1.8793852415718f + in_Renamed[8] * 1.532088886238f + in_Renamed[16] * 0.34729635533386f;
                tmp1 = i00 + in_Renamed[4] - in_Renamed[8] - in_Renamed[12] - in_Renamed[12] - in_Renamed[16];
                tmp2 = iip12 - in_Renamed[4] * 0.34729635533386f - in_Renamed[8] * 1.8793852415718f + in_Renamed[16] * 1.532088886238f;
                tmp3 = iip12 - in_Renamed[4] * 1.532088886238f + in_Renamed[8] * 0.34729635533386f - in_Renamed[16] * 1.8793852415718f;
                tmp4 = in_Renamed[0] - in_Renamed[4] + in_Renamed[8] - in_Renamed[12] + in_Renamed[16];

                // 4 points on even indices
                float i66_ = in_Renamed[6] * 1.732050808f; // Sqrt[3]

                tmp0_ = in_Renamed[2] * 1.9696155060244f + i66_ + in_Renamed[10] * 1.2855752193731f + in_Renamed[14] * 0.68404028665134f;
                tmp1_ = (in_Renamed[2] - in_Renamed[10] - in_Renamed[14]) * 1.732050808f;
                tmp2_ = in_Renamed[2] * 1.2855752193731f - i66_ - in_Renamed[10] * 0.68404028665134f + in_Renamed[14] * 1.9696155060244f;
                tmp3_ = in_Renamed[2] * 0.68404028665134f - i66_ + in_Renamed[10] * 1.9696155060244f - in_Renamed[14] * 1.2855752193731f;

                // 9 point IDCT on odd indices
                // 5 points on odd indices (not realy an IDCT)
                float i0 = in_Renamed[0 + 1] + in_Renamed[0 + 1];
                float i0p12 = i0 + in_Renamed[12 + 1];

                tmp0o = i0p12 + in_Renamed[4 + 1] * 1.8793852415718f + in_Renamed[8 + 1] * 1.532088886238f + in_Renamed[16 + 1] * 0.34729635533386f;
                tmp1o = i0 + in_Renamed[4 + 1] - in_Renamed[8 + 1] - in_Renamed[12 + 1] - in_Renamed[12 + 1] - in_Renamed[16 + 1];
                tmp2o = i0p12 - in_Renamed[4 + 1] * 0.34729635533386f - in_Renamed[8 + 1] * 1.8793852415718f + in_Renamed[16 + 1] * 1.532088886238f;
                tmp3o = i0p12 - in_Renamed[4 + 1] * 1.532088886238f + in_Renamed[8 + 1] * 0.34729635533386f - in_Renamed[16 + 1] * 1.8793852415718f;
                tmp4o = (in_Renamed[0 + 1] - in_Renamed[4 + 1] + in_Renamed[8 + 1] - in_Renamed[12 + 1] + in_Renamed[16 + 1]) * 0.707106781f; // Twiddled

                // 4 points on even indices
                float i6_ = in_Renamed[6 + 1] * 1.732050808f; // Sqrt[3]

                tmp0_o = in_Renamed[2 + 1] * 1.9696155060244f + i6_ + in_Renamed[10 + 1] * 1.2855752193731f + in_Renamed[14 + 1] * 0.68404028665134f;
                tmp1_o = (in_Renamed[2 + 1] - in_Renamed[10 + 1] - in_Renamed[14 + 1]) * 1.732050808f;
                tmp2_o = in_Renamed[2 + 1] * 1.2855752193731f - i6_ - in_Renamed[10 + 1] * 0.68404028665134f + in_Renamed[14 + 1] * 1.9696155060244f;
                tmp3_o = in_Renamed[2 + 1] * 0.68404028665134f - i6_ + in_Renamed[10 + 1] * 1.9696155060244f - in_Renamed[14 + 1] * 1.2855752193731f;

                // Twiddle factors on odd indices
                // and
                // Butterflies on 9 point IDCT's
                // and
                // twiddle factors for 36 point IDCT

                float e, o;
                e = tmp0 + tmp0_; o = (tmp0o + tmp0_o) * 0.501909918f; tmpf_0 = e + o; tmpf_17 = e - o;
                e = tmp1 + tmp1_; o = (tmp1o + tmp1_o) * 0.517638090f; tmpf_1 = e + o; tmpf_16 = e - o;
                e = tmp2 + tmp2_; o = (tmp2o + tmp2_o) * 0.551688959f; tmpf_2 = e + o; tmpf_15 = e - o;
                e = tmp3 + tmp3_; o = (tmp3o + tmp3_o) * 0.610387294f; tmpf_3 = e + o; tmpf_14 = e - o;
                tmpf_4 = tmp4 + tmp4o; tmpf_13 = tmp4 - tmp4o;
                e = tmp3 - tmp3_; o = (tmp3o - tmp3_o) * 0.871723397f; tmpf_5 = e + o; tmpf_12 = e - o;
                e = tmp2 - tmp2_; o = (tmp2o - tmp2_o) * 1.183100792f; tmpf_6 = e + o; tmpf_11 = e - o;
                e = tmp1 - tmp1_; o = (tmp1o - tmp1_o) * 1.931851653f; tmpf_7 = e + o; tmpf_10 = e - o;
                e = tmp0 - tmp0_; o = (tmp0o - tmp0_o) * 5.736856623f; tmpf_8 = e + o; tmpf_9 = e - o;

                // end 36 point IDCT */
                // shift to modified IDCT
                win_bt = win[block_type];

                out_Renamed[0] = -tmpf_9 * win_bt[0];
                out_Renamed[1] = -tmpf_10 * win_bt[1];
                out_Renamed[2] = -tmpf_11 * win_bt[2];
                out_Renamed[3] = -tmpf_12 * win_bt[3];
                out_Renamed[4] = -tmpf_13 * win_bt[4];
                out_Renamed[5] = -tmpf_14 * win_bt[5];
                out_Renamed[6] = -tmpf_15 * win_bt[6];
                out_Renamed[7] = -tmpf_16 * win_bt[7];
                out_Renamed[8] = -tmpf_17 * win_bt[8];
                out_Renamed[9] = tmpf_17 * win_bt[9];
                out_Renamed[10] = tmpf_16 * win_bt[10];
                out_Renamed[11] = tmpf_15 * win_bt[11];
                out_Renamed[12] = tmpf_14 * win_bt[12];
                out_Renamed[13] = tmpf_13 * win_bt[13];
                out_Renamed[14] = tmpf_12 * win_bt[14];
                out_Renamed[15] = tmpf_11 * win_bt[15];
                out_Renamed[16] = tmpf_10 * win_bt[16];
                out_Renamed[17] = tmpf_9 * win_bt[17];
                out_Renamed[18] = tmpf_8 * win_bt[18];
                out_Renamed[19] = tmpf_7 * win_bt[19];
                out_Renamed[20] = tmpf_6 * win_bt[20];
                out_Renamed[21] = tmpf_5 * win_bt[21];
                out_Renamed[22] = tmpf_4 * win_bt[22];
                out_Renamed[23] = tmpf_3 * win_bt[23];
                out_Renamed[24] = tmpf_2 * win_bt[24];
                out_Renamed[25] = tmpf_1 * win_bt[25];
                out_Renamed[26] = tmpf_0 * win_bt[26];
                out_Renamed[27] = tmpf_0 * win_bt[27];
                out_Renamed[28] = tmpf_1 * win_bt[28];
                out_Renamed[29] = tmpf_2 * win_bt[29];
                out_Renamed[30] = tmpf_3 * win_bt[30];
                out_Renamed[31] = tmpf_4 * win_bt[31];
                out_Renamed[32] = tmpf_5 * win_bt[32];
                out_Renamed[33] = tmpf_6 * win_bt[33];
                out_Renamed[34] = tmpf_7 * win_bt[34];
                out_Renamed[35] = tmpf_8 * win_bt[35];
            }
        }

        private int counter = 0;
        private const int SSLIMIT = 18;
        private const int SBLIMIT = 32;

        public class SBI
        {
            public int[] l;
            public int[] s;

            public SBI()
            {
                l = new int[23];
                s = new int[14];
            }
            public SBI(int[] thel, int[] thes)
            {
                l = thel;
                s = thes;
            }
        }

        public class gr_info_s
        {
            public int part2_3_length = 0;
            public int big_values = 0;
            public int global_gain = 0;
            public int scalefac_compress = 0;
            public int window_switching_flag = 0;
            public int block_type = 0;
            public int mixed_block_flag = 0;
            public int[] table_select;
            public int[] subblock_gain;
            public int region0_count = 0;
            public int region1_count = 0;
            public int preflag = 0;
            public int scalefac_scale = 0;
            public int count1table_select = 0;

            /// <summary> Dummy Constructor
            /// </summary>
            public gr_info_s()
            {
                table_select = new int[3];
                subblock_gain = new int[3];
            }
        }

        public class temporaire
        {
            public int[] scfsi;
            public gr_info_s[] gr;

            /// <summary> Dummy Constructor
            /// </summary>
            public temporaire()
            {
                scfsi = new int[4];
                gr = new gr_info_s[2];
                gr[0] = new gr_info_s();
                gr[1] = new gr_info_s();
            }
        }

        public class III_side_info_t
        {

            public int main_data_begin = 0;
            public int private_bits = 0;
            public temporaire[] ch;
            /// <summary> Dummy Constructor
            /// </summary>
            public III_side_info_t()
            {
                ch = new temporaire[2];
                ch[0] = new temporaire();
                ch[1] = new temporaire();
            }
        }

        public class temporaire2
        {
            public int[] l; /* [cb] */
            public int[][] s; /* [window][cb] */

            /// <summary> Dummy Constructor
            /// </summary>
            public temporaire2()
            {
                l = new int[23];
                s = new int[3][];
                for (int i = 0; i < 3; i++)
                {
                    s[i] = new int[13];
                }
            }
        }

        private static readonly int[][] slen = { new int[] { 0, 0, 0, 0, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4 }, new int[] { 0, 1, 2, 3, 0, 1, 2, 3, 1, 2, 3, 1, 2, 3, 2, 3 } };
        public static readonly int[] pretab = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 3, 3, 3, 2, 0 };
        private SBI[] sfBandIndex; // Init in the constructor.
        public static readonly float[] two_to_negative_half_pow = new float[] { 1.0000000000e+00f, 7.0710678119e-01f, 5.0000000000e-01f, 3.5355339059e-01f, 2.5000000000e-01f, 1.7677669530e-01f, 1.2500000000e-01f, 8.8388347648e-02f, 6.2500000000e-02f, 4.4194173824e-02f, 3.1250000000e-02f, 2.2097086912e-02f, 1.5625000000e-02f, 1.1048543456e-02f, 7.8125000000e-03f, 5.5242717280e-03f, 3.9062500000e-03f, 2.7621358640e-03f, 1.9531250000e-03f, 1.3810679320e-03f, 9.7656250000e-04f, 6.9053396600e-04f, 4.8828125000e-04f, 3.4526698300e-04f, 2.4414062500e-04f, 1.7263349150e-04f, 1.2207031250e-04f, 8.6316745750e-05f, 6.1035156250e-05f, 4.3158372875e-05f, 3.0517578125e-05f, 2.1579186438e-05f, 1.5258789062e-05f, 1.0789593219e-05f, 7.6293945312e-06f, 5.3947966094e-06f, 3.8146972656e-06f, 2.6973983047e-06f, 1.9073486328e-06f, 1.3486991523e-06f, 9.5367431641e-07f, 6.7434957617e-07f, 4.7683715820e-07f, 3.3717478809e-07f, 2.3841857910e-07f, 1.6858739404e-07f, 1.1920928955e-07f, 8.4293697022e-08f, 5.9604644775e-08f, 4.2146848511e-08f, 2.9802322388e-08f, 2.1073424255e-08f, 1.4901161194e-08f, 1.0536712128e-08f, 7.4505805969e-09f, 5.2683560639e-09f, 3.7252902985e-09f, 2.6341780319e-09f, 1.8626451492e-09f, 1.3170890160e-09f, 9.3132257462e-10f, 6.5854450798e-10f, 4.6566128731e-10f, 3.2927225399e-10f };

        //UPGRADE_NOTE: Final was removed from the declaration of 't_43 '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
        //UPGRADE_NOTE: The initialization of  't_43' was moved to static method 'javazoom.jl.decoder.LayerIIIDecoder'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
        public static readonly float[] t_43;

        static private float[] create_t_43()
        {
            float[] t43 = new float[8192];
            //UPGRADE_NOTE: Final was removed from the declaration of 'd43 '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
            double d43 = (4.0 / 3.0);

            for (int i = 0; i < 8192; i++)
            {
                t43[i] = (float)System.Math.Pow(i, d43);
            }
            return t43;
        }

        public static readonly float[][] io = { new float[] { 1.0000000000e+00f, 8.4089641526e-01f, 7.0710678119e-01f, 5.9460355751e-01f, 5.0000000001e-01f, 4.2044820763e-01f, 3.5355339060e-01f, 2.9730177876e-01f, 2.5000000001e-01f, 2.1022410382e-01f, 1.7677669530e-01f, 1.4865088938e-01f, 1.2500000000e-01f, 1.0511205191e-01f, 8.8388347652e-02f, 7.4325444691e-02f, 6.2500000003e-02f, 5.2556025956e-02f, 4.4194173826e-02f, 3.7162722346e-02f, 3.1250000002e-02f, 2.6278012978e-02f, 2.2097086913e-02f, 1.8581361173e-02f, 1.5625000001e-02f, 1.3139006489e-02f, 1.1048543457e-02f, 9.2906805866e-03f, 7.8125000006e-03f, 6.5695032447e-03f, 5.5242717285e-03f, 4.6453402934e-03f }, new float[] { 1.0000000000e+00f, 7.0710678119e-01f, 5.0000000000e-01f, 3.5355339060e-01f, 2.5000000000e-01f, 1.7677669530e-01f, 1.2500000000e-01f, 8.8388347650e-02f, 6.2500000001e-02f, 4.4194173825e-02f, 3.1250000001e-02f, 2.2097086913e-02f, 1.5625000000e-02f, 1.1048543456e-02f, 7.8125000002e-03f, 5.5242717282e-03f, 3.9062500001e-03f, 2.7621358641e-03f, 1.9531250001e-03f, 1.3810679321e-03f, 9.7656250004e-04f, 6.9053396603e-04f, 4.8828125002e-04f, 3.4526698302e-04f, 2.4414062501e-04f, 1.7263349151e-04f, 1.2207031251e-04f, 8.6316745755e-05f, 6.1035156254e-05f, 4.3158372878e-05f, 3.0517578127e-05f, 2.1579186439e-05f } };
        public static readonly float[] TAN12 = new float[] { 0.0f, 0.26794919f, 0.57735027f, 1.0f, 1.73205081f, 3.73205081f, 9.9999999e10f, -3.73205081f, -1.73205081f, -1.0f, -0.57735027f, -0.26794919f, 0.0f, 0.26794919f, 0.57735027f, 1.0f };
        private static int[][] reorder_table; // SZD: will be generated on demand


        public static int[] reorder(int[] scalefac_band)
        {
            // SZD: converted from LAME
            int j = 0;
            int[] ix = new int[576];
            for (int sfb = 0; sfb < 13; sfb++)
            {
                int start = scalefac_band[sfb];
                int end = scalefac_band[sfb + 1];
                for (int window = 0; window < 3; window++)
                    for (int i = start; i < end; i++)
                        ix[3 * i + window] = j++;
            }
            return ix;
        }

        private static readonly float[] cs = new float[] { 0.857492925712f, 0.881741997318f, 0.949628649103f, 0.983314592492f, 0.995517816065f, 0.999160558175f, 0.999899195243f, 0.999993155067f };
        private static readonly float[] ca = new float[] { -0.5144957554270f, -0.4717319685650f, -0.3133774542040f, -0.1819131996110f, -0.0945741925262f, -0.0409655828852f, -0.0141985685725f, -0.00369997467375f };


        /// <summary>***********************************************************
        /// </summary>
        /*                             INV_MDCT                        */
        /// <summary>***********************************************************
        /// </summary>
        public static readonly float[][] win = {new float[]{- 1.6141214951e-02f, - 5.3603178919e-02f, - 1.0070713296e-01f, - 1.6280817573e-01f, - 4.9999999679e-01f, - 3.8388735032e-01f, - 6.2061144372e-01f, - 1.1659756083e+00f, - 3.8720752656e+00f, - 4.2256286556e+00f, - 1.5195289984e+00f, - 9.7416483388e-01f, - 7.3744074053e-01f, - 1.2071067773e+00f, - 5.1636156596e-01f, - 4.5426052317e-01f, - 4.0715656898e-01f, - 3.6969460527e-01f, - 3.3876269197e-01f, - 3.1242222492e-01f, - 2.8939587111e-01f, - 2.6880081906e-01f, - 5.0000000266e-01f, - 2.3251417468e-01f, - 2.1596714708e-01f, - 2.0004979098e-01f, - 1.8449493497e-01f, - 1.6905846094e-01f, - 1.5350360518e-01f, - 1.3758624925e-01f, - 1.2103922149e-01f, - 2.0710679058e-01f, - 8.4752577594e-02f, - 6.4157525656e-02f, - 4.1131172614e-02f, - 1.4790705759e-02f}, 
			new float[]{- 1.6141214951e-02f, - 5.3603178919e-02f, - 1.0070713296e-01f, - 1.6280817573e-01f, - 4.9999999679e-01f, - 3.8388735032e-01f, - 6.2061144372e-01f, - 1.1659756083e+00f, - 3.8720752656e+00f, - 4.2256286556e+00f, - 1.5195289984e+00f, - 9.7416483388e-01f, - 7.3744074053e-01f, - 1.2071067773e+00f, - 5.1636156596e-01f, - 4.5426052317e-01f, - 4.0715656898e-01f, - 3.6969460527e-01f, - 3.3908542600e-01f, - 3.1511810350e-01f, - 2.9642226150e-01f, - 2.8184548650e-01f, - 5.4119610000e-01f, - 2.6213228100e-01f, - 2.5387916537e-01f, - 2.3296291359e-01f, - 1.9852728987e-01f, - 1.5233534808e-01f, - 9.6496400054e-02f, - 3.3423828516e-02f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f}, new float[]{- 4.8300800645e-02f, - 1.5715656932e-01f, - 2.8325045177e-01f, - 4.2953747763e-01f, - 1.2071067795e+00f, - 8.2426483178e-01f, - 1.1451749106e+00f, - 1.7695290101e+00f, - 4.5470225061e+00f, - 3.4890531002e+00f, - 7.3296292804e-01f, - 1.5076514758e-01f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f}, new float[]{0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, 0.0000000000e+00f, - 1.5076513660e-01f, - 7.3296291107e-01f, - 3.4890530566e+00f, - 4.5470224727e+00f, - 1.7695290031e+00f, - 1.1451749092e+00f, - 8.3137738100e-01f, - 1.3065629650e+00f, - 5.4142014250e-01f, - 4.6528974900e-01f, - 4.1066990750e-01f, - 3.7004680800e-01f, - 3.3876269197e-01f, - 3.1242222492e-01f, - 2.8939587111e-01f, - 2.6880081906e-01f, - 5.0000000266e-01f, - 2.3251417468e-01f, - 2.1596714708e-01f, - 2.0004979098e-01f, 
- 1.8449493497e-01f, - 1.6905846094e-01f, - 1.5350360518e-01f, - 1.3758624925e-01f, - 1.2103922149e-01f, - 2.0710679058e-01f, - 8.4752577594e-02f, - 6.4157525656e-02f, - 4.1131172614e-02f, - 1.4790705759e-02f}};
        /// <summary>***********************************************************
        /// </summary>
        /*                         END OF INV_MDCT                     */
        /// <summary>***********************************************************
        /// </summary>

        //UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'Sftable' to access its enclosing instance. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1019"'
        public class Sftable
        {
            private void InitBlock(LayerIIIDecoder enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private LayerIIIDecoder enclosingInstance;
            public LayerIIIDecoder Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }

            }
            public int[] l;
            public int[] s;

            public Sftable(LayerIIIDecoder enclosingInstance)
            {
                InitBlock(enclosingInstance);
                l = new int[5];
                s = new int[3];
            }

            public Sftable(LayerIIIDecoder enclosingInstance, int[] thel, int[] thes)
            {
                InitBlock(enclosingInstance);
                l = thel;
                s = thes;
            }
        }

        public Sftable sftable;

        public static readonly int[][][] nr_of_sfb_block = { new int[][] { new int[] { 6, 5, 5, 5 }, new int[] { 9, 9, 9, 9 }, new int[] { 6, 9, 9, 9 } }, new int[][] { new int[] { 6, 5, 7, 3 }, new int[] { 9, 9, 12, 6 }, new int[] { 6, 9, 12, 6 } }, new int[][] { new int[] { 11, 10, 0, 0 }, new int[] { 18, 18, 0, 0 }, new int[] { 15, 18, 0, 0 } }, new int[][] { new int[] { 7, 7, 7, 0 }, new int[] { 12, 12, 12, 0 }, new int[] { 6, 15, 12, 0 } }, new int[][] { new int[] { 6, 6, 6, 3 }, new int[] { 12, 9, 9, 6 }, new int[] { 6, 12, 9, 6 } }, new int[][] { new int[] { 8, 8, 5, 0 }, new int[] { 15, 12, 9, 0 }, new int[] { 6, 18, 9, 0 } } };
        static LayerIIIDecoder()
        {
            t_43 = create_t_43();
        }
    }
}